$(document).ready(function () {
    loadPaymentOrderDetails();

    $("#payNowBtn").click(function () {
        processPayNow();
    });

    $("#cancelPaymentBtn").click(function () {
        processCancelPayment();
    });
});

var currentOrderTotal = 0;

function loadPaymentOrderDetails() {
    var customerId = $("#payment-card").attr("data-customer-id");
    var orderId = $("#payment-card").attr("data-order-id");

    if (!customerId || !orderId) {
        $("#payment-loading").text("Invalid order information. Please navigate from checkout.");
        return;
    }

    apiRequest({
        url: "/api/OrderApi/" + customerId + "/" + orderId,
        type: "GET"
    })
    .done(function (order) {
        $("#payment-loading").hide();

        if (!order) {
            $("#payment-loading").text("Order details not found.").show();
            return;
        }

        $("#payment-card").show();
        $("#pay-order-id").text("#" + order.orderID);
        $("#pay-order-date").text(order.orderDate ? new Date(order.orderDate).toLocaleDateString() : "-");

        currentOrderTotal = order.totalAmount || 0;
        var formattedTotal = Number(currentOrderTotal).toFixed(2);
        $("#pay-order-total").text("₹" + formattedTotal);
        $("#btn-pay-amount").text(formattedTotal);
    })
    .fail(function (xhr) {
        console.log("Payment details load error:", xhr);
        $("#payment-loading").text("Unable to load order details for payment.");
    });
}

function processPayNow() {
    var customerId = $("#payment-card").attr("data-customer-id");
    var orderId = parseInt($("#payment-card").attr("data-order-id"));
    var $msg = $("#paymentMessage");
    $msg.removeClass("text-danger").addClass("text-success").text("Processing payment...");

    var $btnPay = $("#payNowBtn");
    var $btnCancel = $("#cancelPaymentBtn");
    $btnPay.prop("disabled", true);
    $btnCancel.prop("disabled", true);

    var txnId = "TXN-ONLINE-" + orderId + "-" + Math.floor(1000 + Math.random() * 9000);

    // 1. Record Successful Payment
    apiRequest({
        url: "/api/PaymentApi",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            orderID: orderId,
            paymentMethod: "Online Payment",
            paymentStatus: "Successful",
            transactionID: txnId,
            amount: currentOrderTotal
        })
    })
    .done(function () {
        // 2. Update OrderStatus to Confirmed
        apiRequest({
            url: "/api/OrderApi/status",
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                orderID: orderId,
                orderStatus: "Confirmed"
            })
        })
        .always(function () {
            window.location.href = "/Order/Success?orderId=" + orderId;
        });
    })
    .fail(function (xhr) {
        $btnPay.prop("disabled", false);
        $btnCancel.prop("disabled", false);
        $msg.removeClass("text-success").addClass("text-danger").text("Payment failed. Please try again.");
    });
}

function processCancelPayment() {
    var orderId = parseInt($("#payment-card").attr("data-order-id"));
    var $msg = $("#paymentMessage");

    var $btnPay = $("#payNowBtn");
    var $btnCancel = $("#cancelPaymentBtn");
    $btnPay.prop("disabled", true);
    $btnCancel.prop("disabled", true);

    var txnId = "TXN-CANCELLED-" + orderId;

    // Record Failed/Cancelled Payment
    apiRequest({
        url: "/api/PaymentApi",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            orderID: orderId,
            paymentMethod: "Online Payment",
            paymentStatus: "Failed",
            transactionID: txnId,
            amount: currentOrderTotal
        })
    })
    .always(function () {
        $msg.removeClass("text-success").addClass("text-danger")
            .html('Payment was cancelled. Your order remains pending.<br><a href="/Order/Index" class="btn btn-sm btn-outline-secondary mt-2">Go to My Orders</a>');
    });
}
