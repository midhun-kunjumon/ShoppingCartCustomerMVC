$(document).ready(function () {
    var urlParams = new URLSearchParams(window.location.search);
    var returnUrl = urlParams.get("returnUrl") || $("#loginForm").attr("data-return-url") || "";

    $("#loginForm").submit(function (e) {
        e.preventDefault();

        var loginData = {
            email: $("#Email").val(),
            password: $("#Password").val()
        };

        $.ajax({
            url: "/api/AuthApi/login",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(loginData),

            success: function (response) {
                $.ajax({
                    url: "/Account/CompleteLogin",
                    type: "POST",
                    data: {
                        token: response.accessToken,
                        refreshToken: response.refreshToken,
                        returnUrl: returnUrl
                    },

                    success: function (completeRes) {
                        var destUrl = (completeRes && completeRes.returnUrl) ? completeRes.returnUrl : (returnUrl || "/Home/Index");
                        var customerId = (completeRes && completeRes.customerId) ? completeRes.customerId : null;

                        mergeAnonymousData(customerId, function () {
                            window.location.href = destUrl;
                        });
                    },

                    error: function () {
                        $("#loginMessage")
                            .removeClass("login-success")
                            .addClass("login-error")
                            .text("Login failed.");
                    }
                });
            },

            error: function (xhr) {
                console.log("Login error:", xhr);
                $("#loginMessage")
                    .removeClass("login-success")
                    .addClass("login-error")
                    .text("Invalid email or password.");
            }
        });
    });

    $("#googleLoginButton").click(function () {
        var googleUrl = "/Account/GoogleLogin";
        if (returnUrl) {
            googleUrl += "?returnUrl=" + encodeURIComponent(returnUrl);
        }
        window.location.href = googleUrl;
    });
});
