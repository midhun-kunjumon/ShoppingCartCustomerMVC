$(document).ready(function () {

    loadProductsBySubCategory();

    $(document).on("click", ".product-card", function (e) {

        if ($(e.target).closest(".add-to-cart-btn").length) {
            return;
        }

        var productId = $(this).data("product-id");

        if (!productId) {
            return;
        }

        window.location.href =
            "/Product/Details/" + productId;
    });

    $(document).on("click", ".add-to-cart-btn", function (e) {

        e.stopPropagation();

        var productId = $(this).data("product-id");

        var customerId =
            $("#subcategory-products-container").data("customer-id");

        if (!customerId) {
            if (productId) {
                addAnonymousCartItem(productId, 1);
                alert("Product added to cart.");
            } else {
                alert("Invalid product.");
            }
            return;
        }

        if (!productId) {

            alert("Invalid product.");

            return;
        }

        apiRequest({

            url: "/api/CartApi/add" +
                "?customerId=" + customerId +
                "&productId=" + productId +
                "&quantity=1",

            type: "POST"

        })
            .done(function (response) {

                alert(
                    response.message ||
                    response.Message ||
                    "Product added to cart successfully."
                );

            })
            .fail(function (xhr) {

                console.log("Add to cart error:");
                console.log("Status:", xhr.status);
                console.log("Response:", xhr.responseText);

                if (xhr.status === 401) {

                    alert("Please login to add products to your cart.");

                    return;
                }

                alert(
                    "Unable to add product to cart.\n\n" +
                    "Status: " + xhr.status +
                    "\nResponse: " + xhr.responseText
                );

            });

    });

});


function loadProductsBySubCategory() {

    var container = $('#subcategory-products-container');

    var subCategoryId =
        container.data('subcategory-id');


    if (!subCategoryId) {

        container.html(
            '<div class="text-center py-5">' +
            '<p>Invalid subcategory.</p>' +
            '</div>'
        );

        return;
    }


    $.ajax({

        url: '/api/ProductApi/subcategory/' + subCategoryId,

        type: 'GET',

        success: function (products) {

            container.empty();


            if (!products || products.length === 0) {

                container.html(
                    '<div class="text-center py-5">' +
                    '<p>No products available in this subcategory.</p>' +
                    '</div>'
                );

                return;
            }


            $.each(products, function (index, product) {

                var imageUrl = product.imageUrl
                    ? product.imageUrl
                    : '/images/product-placeholder.png';


                var productCard =

                    '<div class="product-card" ' +
                    'data-product-id="' +
                    product.productID +
                    '">' +

                    '<div class="product-image">' +

                    '<img src="' + imageUrl + '" ' +
                    'alt="' + product.productName + '" />' +

                    '</div>' +

                    '<div class="product-content">' +

                    '<span class="product-label">' +
                    'PRODUCT' +
                    '</span>' +

                    '<h5 class="product-name">' +
                    product.productName +
                    '</h5>' +

                    '<p class="product-description">' +
                    (product.description || '') +
                    '</p>' +

                    '<div class="product-bottom">' +

                    '<div class="product-price">' +
                    '₹' +
                    product.price.toLocaleString('en-IN') +
                    '</div>' +

                    '<button type="button" ' +
                    'class="add-to-cart-btn" ' +
                    'data-product-id="' +
                    product.productID +
                    '">' +
                    '+' +
                    '</button>' +

                    '</div>' +

                    '</div>' +

                    '</div>';


                container.append(productCard);

            });

        },

        error: function (xhr, status, error) {

            console.error(
                'Error loading products:',
                error
            );


            container.html(

                '<div class="text-center py-5">' +
                '<p>Unable to load products.</p>' +
                '</div>'

            );

        }

    });

}