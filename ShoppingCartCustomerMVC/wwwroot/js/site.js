/* =========================================
   CATEGORY BAR NAVIGATION
========================================= */

$(document).on(
    'click',
    '.prelogin-category',
    function (e) {

        e.preventDefault();

        var categoryName =
            $(this).data('category-name');


        if (!categoryName) {

            window.location.href =
                '/Category/Index';

            return;
        }


        $.ajax({

            url: '/api/CategoryApi',

            type: 'GET',

            success: function (categories) {

                if (!categories ||
                    categories.length === 0) {

                    window.location.href =
                        '/Category/Index';

                    return;
                }


                var searchName = categoryName.toLowerCase().trim();

                var aliases = {
                    'mobiles': 'electronics',
                    'beauty': 'beauty & personal care',
                    'home': 'home & furnishing',
                    'appliances': 'home appliances',
                    'toys, baby': 'toys & baby',
                    'auto accessories': 'automotive',
                    'furniture': 'home & furnishing'
                };

                var targetName = aliases[searchName] || searchName;

                var selectedCategory =
                    categories.find(function (category) {
                        return category.categoryName.toLowerCase() === targetName;
                    }) ||
                    categories.find(function (category) {
                        return category.categoryName.toLowerCase().indexOf(targetName) !== -1 ||
                               targetName.indexOf(category.categoryName.toLowerCase()) !== -1;
                    });


                if (!selectedCategory) {

                    window.location.href =
                        '/Category/Index';

                    return;
                }

                var catId = selectedCategory.categoryID || selectedCategory.categoryId;

                window.location.href =
                    '/Category/Index?categoryId=' +
                    catId;

            },

            error: function (xhr, status, error) {

                console.error(
                    'Error loading category:',
                    error
                );

                window.location.href =
                    '/Category/Index';

            }

        });

    }
);

/* =========================================
   ANONYMOUS CART & WISHLIST STORAGE HELPERS
========================================= */

function getAnonymousCart() {
    try {
        var cart = localStorage.getItem("cartly_anonymous_cart");
        return cart ? JSON.parse(cart) : [];
    } catch (e) {
        return [];
    }
}

function saveAnonymousCart(cart) {
    localStorage.setItem("cartly_anonymous_cart", JSON.stringify(cart));
}

function addAnonymousCartItem(productId, quantity) {
    var cart = getAnonymousCart();
    productId = parseInt(productId);
    quantity = parseInt(quantity) || 1;

    var existing = cart.find(function (item) {
        return parseInt(item.productID || item.productId) === productId;
    });

    if (existing) {
        existing.quantity = (parseInt(existing.quantity) || 0) + quantity;
    } else {
        cart.push({ productID: productId, quantity: quantity });
    }

    saveAnonymousCart(cart);
}

function getAnonymousWishlist() {
    try {
        var list = localStorage.getItem("cartly_anonymous_wishlist");
        return list ? JSON.parse(list) : [];
    } catch (e) {
        return [];
    }
}

function saveAnonymousWishlist(list) {
    localStorage.setItem("cartly_anonymous_wishlist", JSON.stringify(list));
}

function addAnonymousWishlistItem(productId) {
    var list = getAnonymousWishlist();
    productId = parseInt(productId);
    if (list.indexOf(productId) === -1) {
        list.push(productId);
    }
    saveAnonymousWishlist(list);
}

function mergeAnonymousData(customerId, callback) {
    if (!customerId) {
        if (typeof callback === "function") callback();
        return;
    }

    var anonCart = getAnonymousCart();
    var anonWishlist = getAnonymousWishlist();

    if ((!anonCart || anonCart.length === 0) && (!anonWishlist || anonWishlist.length === 0)) {
        if (typeof callback === "function") callback();
        return;
    }

    // Clear local storage immediately to prevent concurrent requests on page redirect
    if (anonCart && anonCart.length > 0) {
        localStorage.removeItem("cartly_anonymous_cart");
    }
    if (anonWishlist && anonWishlist.length > 0) {
        localStorage.removeItem("cartly_anonymous_wishlist");
    }

    var cartPromise = $.Deferred();
    var wishlistPromise = $.Deferred();

    if (anonCart && anonCart.length > 0) {
        apiRequest({
            url: "/api/CartApi/merge/" + customerId,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(anonCart)
        })
        .always(function () {
            cartPromise.resolve();
        });
    } else {
        cartPromise.resolve();
    }

    if (anonWishlist && anonWishlist.length > 0) {
        apiRequest({
            url: "/api/WishlistApi/merge/" + customerId,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(anonWishlist)
        })
        .always(function () {
            wishlistPromise.resolve();
        });
    } else {
        wishlistPromise.resolve();
    }

    $.when(cartPromise, wishlistPromise).always(function () {
        if (typeof callback === "function") {
            callback();
        }
    });
}

$(document).ready(function () {
    var currentCustomerId = $("body").attr("data-customer-id") ||
                            $("#cart-content, #wishlist-content, #details-content, #checkout-content, #payment-card").attr("data-customer-id");

    if (currentCustomerId) {
        var anonCart = getAnonymousCart();
        var anonWishlist = getAnonymousWishlist();
        if ((anonCart && anonCart.length > 0) || (anonWishlist && anonWishlist.length > 0)) {
            mergeAnonymousData(currentCustomerId, function () {
                if (typeof loadCart === "function") loadCart();
                if (typeof loadWishlist === "function") loadWishlist();
            });
        }
    }
});