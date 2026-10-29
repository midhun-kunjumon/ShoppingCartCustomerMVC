$(document).ready(function () {
    loadCheckoutData();

    $("#placeOrderBtn").click(function () {
        placeOrder();
    });
});

var checkoutCartItems = [];
var grandTotal = 0;

function loadCheckoutData() {
    var customerId = $("#checkout-content").attr("data-customer-id");

    if (!customerId) {
        $("#checkout-loading").text("Unable to identify customer. Please login.");
        return;
    }

    // 1. Fetch Customer Info
    apiRequest({
        url: "/api/CustomerApi/" + customerId,
        type: "GET"
    })
    .done(function (customer) {
        $("#cust-name").val(customer.customerName || "");
        $("#cust-email").val(customer.email || "");
        $("#cust-phone").val(customer.phone || "");
        $("#cust-city").val(customer.city || "");
    });

    // 2. Fetch Cart Items
    apiRequest({
        url: "/api/CartApi/" + customerId,
        type: "GET"
    })
    .done(function (cartItems) {
        $("#checkout-loading").hide();

        if (!cartItems || cartItems.length === 0) {
            $("#checkout-empty").show();
            $("#checkout-content").hide();
            return;
        }

        checkoutCartItems = cartItems;
        $("#checkout-empty").hide();
        $("#checkout-content").show();

        renderCheckoutItems(cartItems);
    })
    .fail(function (xhr) {
        console.log("Checkout loading error:", xhr);
        $("#checkout-loading").text("Failed to load cart items for checkout.");
    });
}

function renderCheckoutItems(items) {
    var $container = $("#checkout-items-container");
    $container.empty();

    grandTotal = 0;

    $.each(items, function (index, item) {
        var itemTotal = item.total || (item.quantity * item.price);
        grandTotal += itemTotal;

        var imgUrl = item.imageUrl || "/images/product-placeholder.png";

        var html = `
            <div class="checkout-item">
                <img src="${imgUrl}" alt="${escapeHtml(item.productName)}" class="checkout-item-img" />
                <div class="checkout-item-details">
                    <div class="checkout-item-name">${escapeHtml(item.productName)}</div>
                    <div class="checkout-item-qty">Quantity: ${item.quantity} × ₹${Number(item.price).toFixed(2)}</div>
                </div>
                <div class="checkout-item-total">
                    ₹${Number(itemTotal).toFixed(2)}
                </div>
            </div>
        `;
        $container.append(html);
    });

    $("#summary-subtotal").text("₹" + grandTotal.toFixed(2));
    $("#summary-total").text("₹" + grandTotal.toFixed(2));

    var expDate = new Date();
    expDate.setDate(expDate.getDate() + 3);
    $("#checkout-expected-delivery").text(
        expDate.toLocaleDateString("en-US", { month: "long", day: "numeric", year: "numeric" })
    );
}

function placeOrder() {
    var customerId = parseInt($("#checkout-content").attr("data-customer-id"));
    var paymentMethod = $('input[name="PaymentMethod"]:checked').val() || "Cash on Delivery";
    var $msg = $("#checkoutMessage");
    $msg.text("");

    if (!customerId) {
        alert("Customer session invalid. Please login again.");
        return;
    }

    if (!checkoutCartItems || checkoutCartItems.length === 0) {
        $msg.text("Your cart is empty.");
        return;
    }

    var $btn = $("#placeOrderBtn");
    $btn.prop("disabled", true).text("Processing Order...");

    var isOnline = (paymentMethod === "Online Payment");
    var initialOrderStatus = isOnline ? "Pending" : "Confirmed";

    // Create Order
    apiRequest({
        url: "/api/OrderApi",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            customerID: customerId,
            paymentMethod: paymentMethod,
            orderStatus: initialOrderStatus
        })
    })
    .done(function (orderResponse) {
        if (!orderResponse.orderID) {
            $btn.prop("disabled", false).text("Place Order");
            $msg.text(orderResponse.message || "Failed to create order.");
            return;
        }

        var orderId = orderResponse.orderID;

        if (isOnline) {
            // Redirect to Online Payment Simulation page
            window.location.href = "/Order/Payment/" + orderId;
        } else {
            // Cash on Delivery flow: create pending payment record and go to Order Success
            var txnId = "COD-" + orderId + "-" + Math.floor(1000 + Math.random() * 9000);
            apiRequest({
                url: "/api/PaymentApi",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    orderID: orderId,
                    paymentMethod: "Cash on Delivery",
                    paymentStatus: "Pending",
                    transactionID: txnId,
                    amount: grandTotal
                })
            })
            .always(function () {
                window.location.href = "/Order/Success?orderId=" + orderId;
            });
        }
    })
    .fail(function (xhr) {
        $btn.prop("disabled", false).text("Place Order");
        console.log("Create order error:", xhr);
        var errText = "Unable to place order. Please try again.";
        if (xhr.responseJSON && xhr.responseJSON.message) {
            errText = xhr.responseJSON.message;
        }
        $msg.text(errText);
    });
}

function escapeHtml(value) {
    return $("<div>").text(value || "").html();
}
