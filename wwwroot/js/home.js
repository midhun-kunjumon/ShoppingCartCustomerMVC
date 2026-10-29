$(document).ready(function () {

    loadFeaturedProducts();

    setupHomeCategoryNavigation();

    setupViewAllCategories();

    setupFeaturedProductEvents();

});


/* =========================================
   FEATURED PRODUCTS
========================================= */

function loadFeaturedProducts() {

    $.ajax({

        url: '/api/ProductApi',

        type: 'GET',

        success: function (products) {

            if (!products || products.length === 0) {
                return;
            }

            var featuredProducts = products.slice(0, 4);

            var cards =
                $('#featured-products-container .product-card');


            $.each(
                featuredProducts,
                function (index, product) {

                    var card = cards.eq(index);
                    var productId = product.productID || product.productId;

                    card.attr('data-product-id', productId);

                    card.find('.product-img')
                        .attr(
                            'src',
                            product.imageUrl || '/images/product-placeholder.png'
                        )
                        .attr(
                            'alt',
                            product.productName
                        );


                    card.find('.product-name')
                        .text(product.productName);


                    card.find('.product-description')
                        .text(
                            product.description ||
                            'No description available.'
                        );


                    card.find('.product-price')
                        .text(
                            '₹' +
                            product.price.toLocaleString('en-IN')
                        );

                    card.find('.cart-button')
                        .attr('data-product-id', productId);

                }
            );

        },

        error: function (xhr, status, error) {

            console.error(
                'Error loading products:',
                error
            );

        }

    });

}


function setupFeaturedProductEvents() {

    /*
     * Featured Product Card Click -> Product Details Page
     */
    $(document)
        .off('click', '#featured-products-container .product-card')
        .on('click', '#featured-products-container .product-card', function (e) {

            if ($(e.target).closest('.cart-button').length > 0) {
                return;
            }

            var productId = $(this).attr('data-product-id');

            if (productId) {
                window.location.href = '/Product/Details/' + productId;
            }

        });


    /*
     * Add to Cart "+" Button Click
     */
    $(document)
        .off('click', '#featured-products-container .cart-button')
        .on('click', '#featured-products-container .cart-button', function (e) {

            e.stopPropagation();
            e.preventDefault();

            var productId = $(this).attr('data-product-id') ||
                $(this).closest('.product-card').attr('data-product-id');

            var customerId = $('#featured-products-container').attr('data-customer-id');


            if (!customerId) {
                if (productId) {
                    addAnonymousCartItem(productId, 1);
                    alert('Product added to cart.');
                } else {
                    alert('Invalid product.');
                }
                return;
            }


            if (!productId) {

                alert('Invalid product.');

                return;
            }

            var $btn = $(this);
            $btn.prop('disabled', true);

            apiRequest({

                url: '/api/CartApi/add' +
                    '?customerId=' + customerId +
                    '&productId=' + productId +
                    '&quantity=1',

                type: 'POST'

            })
                .done(function (response) {

                    alert(
                        response.message ||
                        response.Message ||
                        'Product added to cart successfully.'
                    );

                })
                .fail(function (xhr) {

                    console.error('Add to cart error:', xhr);

                    if (xhr.status === 401) {

                        window.location.href = '/Account/Login';

                        return;
                    }

                    alert(
                        'Unable to add product to cart.\n\n' +
                        'Status: ' + xhr.status +
                        '\nResponse: ' + xhr.responseText
                    );

                })
                .always(function () {

                    $btn.prop('disabled', false);

                });

        });

}


/* =========================================
   HOME CATEGORY NAVIGATION
========================================= */

function setupHomeCategoryNavigation() {

    $(document)
        .off(
            'click',
            '.home-category-card'
        )
        .on(
            'click',
            '.home-category-card',
            function (e) {

                var categoryId =
                    $(this).data('category-id') || $(this).attr('data-category-id');


                if (!categoryId) {

                    console.error(
                        'Category ID not found.'
                    );

                    return;
                }

                e.preventDefault();

                window.location.href =
                    '/Category/Index?categoryId=' +
                    categoryId;

            }
        );

}


/* =========================================
   VIEW ALL CATEGORIES
========================================= */

function setupViewAllCategories() {

    $(document)
        .off(
            'click',
            '#view-all-categories'
        )
        .on(
            'click',
            '#view-all-categories',
            function (e) {

                e.preventDefault();

                window.location.href =
                    '/Category/Index';

            }
        );

}