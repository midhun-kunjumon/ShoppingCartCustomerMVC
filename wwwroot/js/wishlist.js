$(document).ready(function () {
    loadWishlist();
});

function loadWishlist() {
    var customerId = $("#wishlist-content").attr("data-customer-id");

    if (!customerId) {
        loadAnonymousWishlist();
        return;
    }

    apiRequest({
        url: "/api/WishlistApi/" + customerId,
        type: "GET"
    })
    .done(function (items) {
        $("#wishlist-loading").hide();

        if (!items || items.length === 0) {
            $("#wishlist-empty").show();
            $("#wishlist-content").hide();
            return;
        }

        $("#wishlist-empty").hide();
        $("#wishlist-content").show();

        renderWishlist(items);
    })
    .fail(function (xhr) {
        console.log("Wishlist load error:", xhr);
        $("#wishlist-loading").text("Failed to load your wishlist.");
    });
}

function loadAnonymousWishlist() {
    var anonWishlist = getAnonymousWishlist();

    if (!anonWishlist || anonWishlist.length === 0) {
        $("#wishlist-loading").hide();
        $("#wishlist-empty").show();
        $("#wishlist-content").hide();
        return;
    }

    var fetchPromises = [];
    var items = [];

    $.each(anonWishlist, function (index, pId) {
        pId = parseInt(pId);
        var d = $.ajax({
            url: "/api/ProductApi/" + pId,
            type: "GET"
        }).done(function (prod) {
            if (prod) {
                items.push({
                    wishlistID: pId,
                    productID: pId,
                    productName: prod.productName,
                    price: prod.price,
                    imageUrl: prod.imageUrl,
                    addedDate: null
                });
            }
        });

        fetchPromises.push(d);
    });

    $.when.apply($, fetchPromises).always(function () {
        $("#wishlist-loading").hide();

        if (items.length === 0) {
            $("#wishlist-empty").show();
            $("#wishlist-content").hide();
            return;
        }

        $("#wishlist-empty").hide();
        $("#wishlist-content").show();

        renderWishlist(items);
    });
}

function renderWishlist(items) {
    var $container = $("#wishlist-items-container");
    $container.empty();

    $.each(items, function (index, item) {
        var imgUrl = item.imageUrl || "/images/product-placeholder.png";
        var dateFormatted = item.addedDate ? new Date(item.addedDate).toLocaleDateString() : "";

        var html = `
            <div class="wishlist-item" data-wishlist-id="${item.wishlistID}" data-product-id="${item.productID}">
                <img src="${imgUrl}" alt="${escapeHtml(item.productName)}" class="wishlist-item-img" />
                
                <div class="wishlist-item-info">
                    <a href="/Product/Details/${item.productID}" class="wishlist-item-title">
                        ${escapeHtml(item.productName)}
                    </a>
                    <div class="wishlist-item-price">
                        ₹${Number(item.price).toFixed(2)}
                    </div>
                    ${dateFormatted ? `<div class="wishlist-item-date">Added on ${dateFormatted}</div>` : ''}
                </div>

                <div class="wishlist-item-actions">
                    <button type="button" class="add-to-cart-from-wishlist btn-add-cart" data-product-id="${item.productID}">
                        <i class="fa-solid fa-cart-shopping me-1"></i> Add to Cart
                    </button>
                    <button type="button" class="remove-wishlist-btn btn-remove-wishlist" data-wishlist-id="${item.wishlistID}" title="Remove from Wishlist">
                        <i class="fa-solid fa-trash-can"></i>
                    </button>
                </div>
            </div>
        `;
        $container.append(html);
    });

    bindWishlistEvents();
}

function bindWishlistEvents() {
    $(".btn-remove-wishlist").off("click").on("click", function () {
        var wishlistId = $(this).data("wishlist-id");
        removeWishlistItem(wishlistId);
    });

    $(".btn-add-cart").off("click").on("click", function () {
        var productId = $(this).data("product-id");
        addToCartFromWishlist(productId);
    });
}

function removeWishlistItem(wishlistId) {
    var customerId = $("#wishlist-content").attr("data-customer-id");

    if (!customerId) {
        var anonWishlist = getAnonymousWishlist();
        anonWishlist = anonWishlist.filter(function (id) { return parseInt(id) !== parseInt(wishlistId); });
        saveAnonymousWishlist(anonWishlist);
        loadWishlist();
        return;
    }

    apiRequest({
        url: "/api/WishlistApi/remove?customerId=" + customerId + "&wishlistId=" + wishlistId,
        type: "DELETE"
    })
    .done(function () {
        loadWishlist();
    })
    .fail(function (xhr) {
        console.log("Remove wishlist error:", xhr);
        alert("Failed to remove item from wishlist.");
    });
}

function addToCartFromWishlist(productId) {
    var customerId = $("#wishlist-content").attr("data-customer-id");

    if (!customerId) {
        addAnonymousCartItem(productId, 1);
        alert("Product added to cart successfully!");
        return;
    }

    apiRequest({
        url: "/api/CartApi/add?customerId=" + customerId + "&productId=" + productId + "&quantity=1",
        type: "POST"
    })
    .done(function (response) {
        alert("Product added to cart successfully!");
    })
    .fail(function (xhr) {
        console.log("Add to cart error:", xhr);
        alert("Failed to add product to cart.");
    });
}

function escapeHtml(value) {
    return $("<div>").text(value || "").html();
}
