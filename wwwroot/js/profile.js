$(document).ready(function () {
    loadProfile();

    $("#profileForm").submit(function (e) {
        e.preventDefault();
        saveProfile();
    });
});

function loadProfile() {
    var customerId = $("#profile-card").attr("data-customer-id");

    if (!customerId) {
        $("#profile-loading").text("Unable to identify customer. Please login again.");
        return;
    }

    apiRequest({
        url: "/api/CustomerApi/" + customerId,
        type: "GET"
    })
    .done(function (customer) {
        $("#profile-loading").hide();
        $("#profile-card").show();

        $("#CustomerName").val(customer.customerName || "");
        $("#Email").val(customer.email || "");
        $("#Phone").val(customer.phone || "");
        $("#City").val(customer.city || "");

        var name = customer.customerName || "Customer";
        $("#display-name").text(name);
        $("#avatar-circle").text(name.charAt(0).toUpperCase());
        $("#display-auth-provider").text(customer.authProvider || "Local");
    })
    .fail(function (xhr) {
        console.log("Profile load error:", xhr);
        $("#profile-loading").text("Failed to load customer profile details.");
    });
}

function saveProfile() {
    var customerId = $("#profile-card").attr("data-customer-id");
    var customerName = $("#CustomerName").val().trim();
    var email = $("#Email").val().trim();
    var phone = $("#Phone").val().trim();
    var city = $("#City").val().trim();

    var $msg = $("#profileMessage");
    $msg.removeClass("msg-success msg-error").text("");

    if (!customerName) {
        $msg.addClass("msg-error").text("Customer name cannot be empty.");
        return;
    }

    var updateData = {
        customerID: parseInt(customerId),
        customerName: customerName,
        email: email,
        phone: phone || null,
        city: city || null
    };

    var $btn = $("#saveProfileBtn");
    $btn.prop("disabled", true).text("Saving...");

    apiRequest({
        url: "/api/CustomerApi/" + customerId,
        type: "PUT",
        contentType: "application/json",
        data: JSON.stringify(updateData)
    })
    .done(function (response) {
        $btn.prop("disabled", false).text("Save Changes");
        $msg.addClass("msg-success").text("Profile updated successfully!");

        var name = customerName;
        $("#display-name").text(name);
        $("#avatar-circle").text(name.charAt(0).toUpperCase());

        setTimeout(function () {
            $msg.text("");
        }, 3000);
    })
    .fail(function (xhr) {
        $btn.prop("disabled", false).text("Save Changes");
        console.log("Profile update error:", xhr);
        var errorMsg = "Failed to update profile.";
        if (xhr.responseJSON && xhr.responseJSON.message) {
            errorMsg = xhr.responseJSON.message;
        }
        $msg.addClass("msg-error").text(errorMsg);
    });
}
