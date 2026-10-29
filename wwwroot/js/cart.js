$(document).ready(function () {

    $("#clearCartButton").click(function () {
        clearCart();
    });

    $("#checkout-btn").click(function () {
        var customerId = $("#cart-content").data("customer-id") || $("#cart-content").attr("data-customer-id");

        if (!customerId) {
            window.location.href = "/Account/Login?returnUrl=/Order/Checkout";
            return;
        }

        window.location.href = "/Order/Checkout";
    });

    loadCart();

});


function loadCart() {

    var customerId = $("#cart-content").data("customer-id") || $("#cart-content").attr("data-customer-id");

    if (!customerId) {
        loadAnonymousCart();
        return;
    }

    apiRequest({
        url: "/api/CartApi/" + customerId,
        type: "GET"
    })
        .done(function (cartItems) {

            $("#cart-loading").hide();

            if (!cartItems || cartItems.length === 0) {

                $("#cart-empty").show();
                $("#cart-content").hide();

                return;
            }

            $("#cart-empty").hide();
            $("#cart-content").show();

            renderCart(cartItems);

        })
        .fail(function (xhr) {

            console.log("Cart loading error:", xhr);

            $("#cart-loading").hide();

            showCartError("Unable to load your cart.");

        });
}


function loadAnonymousCart() {
    var anonCart = getAnonymousCart();

    if (!anonCart || anonCart.length === 0) {
        $("#cart-loading").hide();
        $("#cart-empty").show();
        $("#cart-content").hide();
        return;
    }

    var fetchPromises = [];
    var cartItems = [];

    $.each(anonCart, function (index, item) {
        var pId = parseInt(item.productID || item.productId);
        var qty = parseInt(item.quantity) || 1;

        var d = $.ajax({
            url: "/api/ProductApi/" + pId,
            type: "GET"
        }).done(function (prod) {
            if (prod) {
                cartItems.push({
                    cartItemID: pId,
                    cartID: pId,
                    productID: pId,
                    productName: prod.productName,
                    price: prod.price,
                    quantity: qty,
                    total: prod.price * qty,
                    imageUrl: prod.imageUrl
                });
            }
        });

        fetchPromises.push(d);
    });

    $.when.apply($, fetchPromises).always(function () {
        $("#cart-loading").hide();

        if (cartItems.length === 0) {
            $("#cart-empty").show();
            $("#cart-content").hide();
            return;
        }

        $("#cart-empty").hide();
        $("#cart-content").show();

        renderCart(cartItems);
    });
}


function renderCart(cartItems) {

    var container = $("#cart-items-container");

    container.empty();

    var subtotal = 0;

    $.each(cartItems, function (index, item) {

        var itemTotal = item.total;

        subtotal += itemTotal;

        var productImage = item.imageUrl || "/images/product-placeholder.png";

        var itemHtml = `
            <div class="cart-item"
                 data-cart-item-id="${item.cartItemID}">

                <div class="cart-item-image">

                    <img src="${productImage}"
                         alt="${escapeHtml(item.productName)}" />

                </div>


                <div class="cart-item-info">

                    <h3>
                        ${escapeHtml(item.productName)}
                    </h3>

                    <div class="cart-item-price">
                        ₹${Number(item.price).toFixed(2)}
                    </div>

                </div>


                <div class="cart-item-actions">

                    <div class="quantity-control">

                        <button type="button"
                                class="quantity-btn decrease-quantity"
                                data-cart-item-id="${item.cartItemID}"
                                data-quantity="${item.quantity}">
                            −
                        </button>

                        <span class="quantity-value">
                            ${item.quantity}
                        </span>

                        <button type="button"
                                class="quantity-btn increase-quantity"
                                data-cart-item-id="${item.cartItemID}"
                                data-quantity="${item.quantity}">
                            +
                        </button>

                    </div>


                    <div class="cart-item-total">
                        ₹${Number(itemTotal).toFixed(2)}
                    </div>


                    <button type="button"
                            class="remove-cart-item"
                            data-cart-item-id="${item.cartItemID}">
                        Remove
                    </button>

                </div>

            </div>
        `;

        container.append(itemHtml);

    });


    $("#cart-subtotal").text(
        "₹" + subtotal.toFixed(2)
    );

    $("#cart-total").text(
        "₹" + subtotal.toFixed(2)
    );

    var expDate = new Date();
    expDate.setDate(expDate.getDate() + 3);
    $("#cart-expected-delivery").text(
        expDate.toLocaleDateString("en-US", { month: "long", day: "numeric", year: "numeric" })
    );


    bindCartEvents();

}


function bindCartEvents() {

    $(".increase-quantity").off("click").on("click", function () {

        var cartItemId = $(this).data("cart-item-id");
        var quantity = $(this).data("quantity");

        updateCartQuantity(
            cartItemId,
            quantity + 1
        );

    });


    $(".decrease-quantity").off("click").on("click", function () {

        var cartItemId = $(this).data("cart-item-id");
        var quantity = $(this).data("quantity");

        if (quantity <= 1) {
            return;
        }

        updateCartQuantity(
            cartItemId,
            quantity - 1
        );

    });


    $(".remove-cart-item").off("click").on("click", function () {

        var cartItemId = $(this).data("cart-item-id");

        removeCartItem(cartItemId);

    });

}


function updateCartQuantity(cartItemId, quantity) {

    var customerId = $("#cart-content").attr("data-customer-id");

    cartItemId = parseInt(cartItemId);
    quantity = parseInt(quantity);

    if (!customerId) {
        var anonCart = getAnonymousCart();
        var item = anonCart.find(function (i) { return parseInt(i.productID || i.productId) === cartItemId; });
        if (item) {
            item.quantity = quantity;
            saveAnonymousCart(anonCart);
        }
        loadCart();
        return;
    }

    customerId = parseInt(customerId);

    if (!cartItemId || !quantity) {
        alert("Invalid cart information.");
        return;
    }

    apiRequest({
        url: "/api/CartApi/update?customerId=" +
            customerId +
            "&cartItemId=" +
            cartItemId +
            "&quantity=" +
            quantity,

        type: "PUT"
    })
        .done(function () {

            loadCart();

        })
        .fail(function (xhr) {

            console.log("Cart update error:");
            console.log("Status:", xhr.status);
            console.log("Response:", xhr.responseText);

            alert(
                "Unable to update cart.\n\n" +
                "Status: " + xhr.status +
                "\nResponse: " + xhr.responseText
            );

        });
}


function removeCartItem(cartItemId) {

    var customerId = $("#cart-content").attr("data-customer-id");

    cartItemId = parseInt(cartItemId);

    if (!customerId) {
        var anonCart = getAnonymousCart();
        anonCart = anonCart.filter(function (i) { return parseInt(i.productID || i.productId) !== cartItemId; });
        saveAnonymousCart(anonCart);
        loadCart();
        return;
    }

    customerId = parseInt(customerId);

    if (!cartItemId) {
        alert("Invalid cart information.");
        return;
    }

    apiRequest({
        url: "/api/CartApi/remove?customerId=" +
            customerId +
            "&cartItemId=" +
            cartItemId,

        type: "DELETE"
    })
        .done(function () {

            loadCart();

        })
        .fail(function (xhr) {

            console.log("Cart remove error:");
            console.log("Status:", xhr.status);
            console.log("Response:", xhr.responseText);

            alert(
                "Unable to remove item.\n\n" +
                "Status: " + xhr.status +
                "\nResponse: " + xhr.responseText
            );

        });
}


function showCartError(message) {

    $("#cart-loading")
        .text(message)
        .show();

}


function escapeHtml(value) {

    return $("<div>")
        .text(value || "")
        .html();

}

function clearCart() {

    var customerId = $("#cart-content").attr("data-customer-id");

    if (!confirm("Are you sure you want to clear your cart?")) {
        return;
    }

    if (!customerId) {
        localStorage.removeItem("cartly_anonymous_cart");
        loadCart();
        return;
    }

    customerId = parseInt(customerId);

    apiRequest({
        url: "/api/CartApi/clear/" + customerId,
        type: "DELETE"
    })
        .done(function () {

            loadCart();

        })
        .fail(function (xhr) {

            console.log("Clear cart error:");
            console.log("Status:", xhr.status);
            console.log("Response:", xhr.responseText);

            alert(
                "Unable to clear cart.\n\n" +
                "Status: " + xhr.status +
                "\nResponse: " + xhr.responseText
            );

        });
}