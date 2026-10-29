$(document).ready(function () {
    loadProductDetails();
});

function loadProductDetails() {
    var container = $('#product-details-container');
    var productId = container.attr('data-product-id') || container.data('product-id');

    if (!productId) {
        showProductNotFound();
        return;
    }

    $.ajax({
        url: '/api/ProductApi/' + productId,
        type: 'GET',
        success: function (product) {
            if (!product) {
                showProductNotFound();
                return;
            }

            var pId = product.productID || product.productId;
            var imageUrl = product.imageUrl ? product.imageUrl : '/images/product-placeholder.png';
            var stockText = product.stock > 0 ? 'In Stock' : 'Out of Stock';

            var expDate = new Date();
            expDate.setDate(expDate.getDate() + 3);
            var expDateFormatted = expDate.toLocaleDateString("en-US", { month: "long", day: "numeric", year: "numeric" });

            var productHtml =
                '<div class="product-details-wrapper">' +
                '<div class="product-details-image">' +
                '<img src="' + imageUrl + '" alt="' + (product.productName || '') + '" />' +
                '</div>' +

                '<div class="product-details-content">' +
                '<span class="product-details-label">PRODUCT</span>' +
                '<h1 class="product-details-name">' + (product.productName || '') + '</h1>' +
                '<p class="product-details-brand">' + (product.brand || '') + '</p>' +

                '<div class="product-details-price">₹' + Number(product.price || 0).toLocaleString('en-IN') + '</div>' +

                '<p class="product-details-description">' + (product.description || 'No description available.') + '</p>' +

                '<div class="product-details-stock ' + (product.stock > 0 ? 'in-stock' : 'out-of-stock') + '">' +
                stockText +
                '</div>' +

                '<div class="product-details-delivery my-3 text-dark d-flex align-items-center" style="background:#f8f9fa; border:1px solid #e2e8f0; border-radius:8px; padding:12px 16px; font-size:0.95rem; max-width:350px;">' +
                '<i class="fa-solid fa-truck-fast text-primary me-3" style="font-size:1.4rem;"></i>' +
                '<div><span class="text-muted d-block" style="font-size:0.75rem; font-weight:700; text-transform:uppercase; letter-spacing:0.5px;">Expected Delivery</span>' +
                '<strong class="text-dark" style="font-size:1rem;">' + expDateFormatted + '</strong></div>' +
                '</div>' +

                '<div class="product-details-actions">' +
                '<button type="button" class="add-to-cart-details-btn" data-product-id="' + pId + '"' + (product.stock <= 0 ? ' disabled' : '') + '>' +
                'Add to Cart' +
                '</button>' +
                '<button type="button" class="add-to-wishlist-details-btn ms-2" style="background-color:#fff; color:#2874f0; border:1px solid #2874f0; padding:12px 24px; font-weight:600; border-radius:4px; cursor:pointer;" data-product-id="' + pId + '">' +
                '<i class="fa-regular fa-heart me-1"></i> Add to Wishlist' +
                '</button>' +
                '</div>' +
                '</div>' +
                '</div>';

            container.html(productHtml);

            $('.add-to-cart-details-btn').off('click').on('click', function () {
                var targetId = parseInt($(this).attr('data-product-id') || $(this).data('product-id') || pId);
                addProductToCart(targetId);
            });

            $('.add-to-wishlist-details-btn').off('click').on('click', function () {
                var targetId = parseInt($(this).attr('data-product-id') || $(this).data('product-id') || pId);
                addProductToWishlist(targetId);
            });

            loadRelatedProducts(product);
        },
        error: function (xhr, status, error) {
            console.error('Error loading product details:', error);
            showProductNotFound();
        }
    });
}

function addProductToCart(productId) {
    productId = parseInt(productId);

    if (!productId || isNaN(productId)) {
        alert("Invalid product.");
        return;
    }

    var customerId = $('#product-details-container').attr('data-customer-id') || $('body').attr('data-customer-id');

    if (!customerId || customerId === "" || customerId === "0" || customerId === "undefined") {
        addAnonymousCartItem(productId, 1);
        alert("Product added to cart.");
        return;
    }

    var parsedCustomerId = parseInt(customerId);
    if (!parsedCustomerId || isNaN(parsedCustomerId)) {
        addAnonymousCartItem(productId, 1);
        alert("Product added to cart.");
        return;
    }

    var button = $('.add-to-cart-details-btn');
    button.prop('disabled', true);

    apiRequest({
        url: '/api/CartApi/add?customerId=' + parsedCustomerId + '&productId=' + productId + '&quantity=1',
        type: 'POST'
    })
    .done(function (response) {
        alert(response.message || 'Product added to cart successfully.');
    })
    .fail(function (xhr) {
        console.log('Add to cart error:', xhr);
        if (xhr.status === 401) {
            addAnonymousCartItem(productId, 1);
            alert("Product added to cart.");
            return;
        }
        alert('Unable to add product to cart.');
    })
    .always(function () {
        button.prop('disabled', false);
    });
}

function loadRelatedProducts(currentProduct) {
    $.ajax({
        url: '/api/ProductApi',
        type: 'GET',
        success: function (products) {
            if (!products || products.length === 0) {
                renderRelatedProducts([]);
                return;
            }

            var currId = currentProduct.productID || currentProduct.productId;
            var currCatId = currentProduct.categoryID || currentProduct.categoryId;

            var relatedProducts = products.filter(function (product) {
                var pCatId = product.categoryID || product.categoryId;
                var pId = product.productID || product.productId;
                return pCatId === currCatId && pId !== currId;
            });

            renderRelatedProducts(relatedProducts);
        },
        error: function (xhr, status, error) {
            console.error('Error loading related products:', error);
            $('#related-products-container').html('<p class="no-related-products">Unable to load related products.</p>');
        }
    });
}

function renderRelatedProducts(products) {
    var container = $('#related-products-container');

    if (!products || products.length === 0) {
        container.html('<p class="no-related-products">No related products available.</p>');
        return;
    }

    var cards = '';

    $.each(products, function (index, product) {
        var pId = product.productID || product.productId;
        var imageUrl = product.imageUrl ? product.imageUrl : '/images/product-placeholder.png';

        cards +=
            '<div class="related-product-card" data-product-id="' + pId + '">' +
            '<div class="related-product-image">' +
            '<img src="' + imageUrl + '" alt="' + (product.productName || '') + '" />' +
            '</div>' +
            '<div class="related-product-content">' +
            '<h3>' + (product.productName || '') + '</h3>' +
            '<p class="related-product-brand">' + (product.brand || '') + '</p>' +
            '<div class="related-product-bottom">' +
            '<span class="related-product-price">₹' + Number(product.price || 0).toLocaleString('en-IN') + '</span>' +
            '</div>' +
            '</div>' +
            '</div>';
    });

    container.html(cards);

    $('.related-prev').off('click').on('click', function () {
        container.animate({ scrollLeft: container.scrollLeft() - 300 }, 300);
    });

    $('.related-next').off('click').on('click', function () {
        container.animate({ scrollLeft: container.scrollLeft() + 300 }, 300);
    });

    $('.related-product-card').off('click').on('click', function () {
        var productId = $(this).attr('data-product-id') || $(this).data('product-id');
        window.location.href = '/Product/Details/' + productId;
    });
}

function showProductNotFound() {
    $('#product-details-container').html(
        '<div class="text-center py-5">' +
        '<h4>Product not found.</h4>' +
        '<p class="text-muted">The requested product could not be found.</p>' +
        '</div>'
    );
}

function addProductToWishlist(productId) {
    productId = parseInt(productId);

    if (!productId || isNaN(productId)) {
        alert("Invalid product.");
        return;
    }

    var customerId = $('#product-details-container').attr('data-customer-id') || $('body').attr('data-customer-id');

    if (!customerId || customerId === "" || customerId === "0" || customerId === "undefined") {
        addAnonymousWishlistItem(productId);
        alert("Product added to wishlist.");
        return;
    }

    var parsedCustomerId = parseInt(customerId);
    if (!parsedCustomerId || isNaN(parsedCustomerId)) {
        addAnonymousWishlistItem(productId);
        alert("Product added to wishlist.");
        return;
    }

    apiRequest({
        url: '/api/WishlistApi/add?customerId=' + parsedCustomerId + '&productId=' + productId,
        type: 'POST'
    })
    .done(function (response) {
        alert(response.message || 'Product added to wishlist successfully.');
    })
    .fail(function (xhr) {
        if (xhr.status === 401) {
            addAnonymousWishlistItem(productId);
            alert("Product added to wishlist.");
            return;
        }
        var msg = "Unable to add product to wishlist.";
        if (xhr.responseJSON && xhr.responseJSON.message) {
            msg = xhr.responseJSON.message;
        }
        alert(msg);
    });
}