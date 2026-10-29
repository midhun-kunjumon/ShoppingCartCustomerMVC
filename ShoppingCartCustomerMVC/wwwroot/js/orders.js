$(document).ready(function () {
    if ($("#orders-content").length > 0) {
        loadOrdersList();
    }

    if ($("#details-content").length > 0) {
        loadOrderDetails();
    }
});

function loadOrdersList() {
    var customerId = $("#orders-content").attr("data-customer-id");

    if (!customerId) {
        $("#orders-loading").text("Unable to identify customer.");
        return;
    }

    apiRequest({
        url: "/api/OrderApi/" + customerId,
        type: "GET"
    })
    .done(function (orders) {
        $("#orders-loading").hide();

        if (!orders || orders.length === 0) {
            $("#orders-empty").show();
            $("#orders-content").hide();
            return;
        }

        $("#orders-empty").hide();
        $("#orders-content").show();

        renderOrdersList(orders);
    })
    .fail(function (xhr) {
        console.log("Orders loading error:", xhr);
        $("#orders-loading").text("Failed to load your orders.");
    });
}

function renderOrdersList(orders) {
    var $container = $("#orders-list-container");
    $container.empty();

    $.each(orders, function (index, order) {
        var dateFormatted = order.orderDate ? new Date(order.orderDate).toLocaleDateString() : "-";
        var expFormatted = "-";
        if (order.expectedDeliveryDate) {
            expFormatted = new Date(order.expectedDeliveryDate).toLocaleDateString();
        } else if (order.orderDate) {
            var d = new Date(order.orderDate);
            d.setDate(d.getDate() + 3);
            expFormatted = d.toLocaleDateString();
        }
        var statusClass = getStatusBadgeClass(order.orderStatus);
        var payStatus = order.paymentStatus || "Pending";
        var payBadgeClass = (payStatus.toLowerCase() === "successful") ? "bg-success" :
                           (payStatus.toLowerCase() === "refund initiated") ? "bg-info text-dark" : "bg-secondary";

        var html = `
            <div class="order-card">
                <div>
                    <div class="order-info-id">Order #${order.orderID}</div>
                    <div class="order-info-date">Placed on ${dateFormatted}</div>
                    <div class="order-info-date text-muted" style="font-size:12px;">Expected Delivery: ${expFormatted}</div>
                </div>

                <div class="d-flex flex-column gap-1 align-items-start">
                    <span class="order-status-badge ${statusClass}">
                        ${escapeHtml(order.orderStatus || "Pending")}
                    </span>
                    <span class="badge ${payBadgeClass}" style="font-size:11px;">
                        Pay: ${escapeHtml(payStatus)}
                    </span>
                </div>

                <div class="order-amount">
                    ₹${Number(order.totalAmount).toFixed(2)}
                </div>

                <div>
                    <a href="/Order/Details/${order.orderID}" class="view-order-btn">
                        View Details
                    </a>
                </div>
            </div>
        `;
        $container.append(html);
    });
}

function loadOrderDetails() {
    var customerId = $("#details-content").attr("data-customer-id");
    var orderId = $("#details-content").attr("data-order-id");

    if (!customerId || !orderId) {
        $("#details-loading").hide();
        $("#details-error").show();
        return;
    }

    apiRequest({
        url: "/api/OrderApi/" + customerId + "/" + orderId,
        type: "GET"
    })
    .done(function (order) {
        $("#details-loading").hide();

        if (!order) {
            $("#details-error").show();
            return;
        }

        $("#details-content").show();

        $("#detail-order-id").text(order.orderID);
        $("#detail-order-date").text(order.orderDate ? new Date(order.orderDate).toLocaleDateString() : "-");

        var statusClass = getStatusBadgeClass(order.orderStatus);
        $("#detail-order-status")
            .attr("class", "order-status-badge " + statusClass)
            .text(order.orderStatus || "Pending");

        $("#detail-cust-name").text(order.customerName || "-");
        $("#detail-cust-email").text(order.email || "-");
        $("#detail-cust-phone").text(order.phone || "-");
        $("#detail-cust-city").text(order.city || "-");

        var expDateText = "-";
        if (order.expectedDeliveryDate) {
            expDateText = new Date(order.expectedDeliveryDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: "numeric" });
        } else if (order.orderDate) {
            var d = new Date(order.orderDate);
            d.setDate(d.getDate() + 3);
            expDateText = d.toLocaleDateString("en-US", { month: "long", day: "numeric", year: "numeric" });
        }
        $("#detail-expected-delivery").text(expDateText);

        var actualDateText = "Not delivered yet";
        if (order.deliveryDate) {
            actualDateText = new Date(order.deliveryDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: "numeric" });
        }
        $("#detail-actual-delivery").text(actualDateText);

        $("#detail-payment-method").text(order.paymentMethod || "Cash on Delivery");
        
        var payStatus = order.paymentStatus || "Pending";
        var payStatusClass = (payStatus.toLowerCase() === "successful") ? "bg-success" : 
                            (payStatus.toLowerCase() === "refund initiated") ? "bg-info text-dark" : "bg-secondary";
        $("#detail-payment-status")
            .attr("class", "badge " + payStatusClass)
            .text(payStatus);

        renderOrderItems(order.orderItems || []);

        $("#detail-summary-subtotal").text("₹" + Number(order.totalAmount).toFixed(2));
        $("#detail-summary-total").text("₹" + Number(order.totalAmount).toFixed(2));

        var currentStatus = (order.orderStatus || "").toLowerCase();
        if (currentStatus === "pending" || currentStatus === "confirmed") {
            $("#cancel-order-container").show();
            bindCancelOrderEvent(customerId, orderId);
        } else {
            $("#cancel-order-container").hide();
        }
    })
    .fail(function (xhr) {
        console.log("Order details load error:", xhr);
        $("#details-loading").hide();
        $("#details-error").show();
    });
}

function bindCancelOrderEvent(customerId, orderId) {
    $("#btnCancelOrder").off("click").on("click", function () {
        if (!confirm("Are you sure you want to cancel this order?")) {
            return;
        }

        var $btn = $(this);
        $btn.prop("disabled", true).text("Cancelling...");

        var $alert = $("#cancelAlertMsg");
        $alert.removeClass("text-danger text-success").text("");

        apiRequest({
            url: "/api/OrderApi/cancel",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                customerID: parseInt(customerId),
                orderID: parseInt(orderId)
            })
        })
        .done(function (response) {
            var msg = response.message || "Order cancelled successfully.";
            $alert.addClass("text-success").text(msg);
            setTimeout(function () {
                loadOrderDetails();
            }, 1500);
        })
        .fail(function (xhr) {
            $btn.prop("disabled", false).html('<i class="fa-solid fa-ban me-1"></i> Cancel Order');
            var errMsg = "Failed to cancel order.";
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errMsg = xhr.responseJSON.message;
            }
            $alert.addClass("text-danger").text(errMsg);
        });
    });
}

function renderOrderItems(items) {
    var $container = $("#detail-items-container");
    $container.empty();

    if (!items || items.length === 0) {
        $container.html('<div class="text-muted py-2">No item details available.</div>');
        return;
    }

    $.each(items, function (index, item) {
        var subtotal = item.subtotal || (item.quantity * item.price);

        var html = `
            <div class="checkout-item">
                <div class="checkout-item-details">
                    <a href="/Product/Details/${item.productID}" class="checkout-item-name text-decoration-none">
                        ${escapeHtml(item.productName)}
                    </a>
                    <div class="checkout-item-qty">
                        Quantity: ${item.quantity} × ₹${Number(item.price).toFixed(2)}
                    </div>
                </div>
                <div class="checkout-item-total">
                    ₹${Number(subtotal).toFixed(2)}
                </div>
            </div>
        `;
        $container.append(html);
    });
}

function getStatusBadgeClass(status) {
    if (!status) return "status-pending";
    status = status.toLowerCase();
    if (status === "confirmed") return "status-completed";
    if (status === "completed" || status === "delivered") return "status-completed";
    if (status === "cancelled") return "status-cancelled";
    return "status-pending";
}

function escapeHtml(value) {
    return $("<div>").text(value || "").html();
}
