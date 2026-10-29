$(document).ready(function () {

    $("#registerForm").submit(function (e) {
        e.preventDefault();

        var customerName = $("#CustomerName").val().trim();
        var email = $("#Email").val().trim();
        var phone = $("#Phone").val().trim();
        var city = $("#City").val().trim();
        var password = $("#Password").val();
        var confirmPassword = $("#ConfirmPassword").val();

        var $msg = $("#registerMessage");
        $msg.removeClass("login-success login-error").text("");

        if (!customerName || !email || !password || !confirmPassword) {
            $msg.addClass("login-error").text("Please fill in all required fields.");
            return;
        }

        if (password !== confirmPassword) {
            $msg.addClass("login-error").text("Passwords do not match.");
            return;
        }

        var registerData = {
            customerName: customerName,
            email: email,
            phone: phone || null,
            city: city || null,
            password: password,
            confirmPassword: confirmPassword
        };

        var $btn = $("#registerSubmitBtn");
        $btn.prop("disabled", true).text("Creating Account...");

        $.ajax({
            url: "/api/CustomerApi/register",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(registerData),
            success: function (response) {
                $msg.addClass("login-success")
                    .text("Account created successfully! Redirecting to Login...");

                setTimeout(function () {
                    window.location.href = "/Account/Login";
                }, 1500);
            },
            error: function (xhr) {
                $btn.prop("disabled", false).text("Create Account");
                var errorText = "Registration failed. Please try again.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorText = xhr.responseJSON.message;
                } else if (xhr.responseText) {
                    try {
                        var parsed = JSON.parse(xhr.responseText);
                        if (parsed.message) errorText = parsed.message;
                    } catch (e) { }
                }
                $msg.addClass("login-error").text(errorText);
            }
        });
    });

});
