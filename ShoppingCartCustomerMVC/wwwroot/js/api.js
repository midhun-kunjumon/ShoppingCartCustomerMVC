function apiRequest(options) {

    var token = sessionStorage.getItem("jwtToken");

    if (!token) {
        token = getCookie("jwtToken");
    }

    if (!options.headers) {
        options.headers = {};
    }

    if (token) {
        options.headers["Authorization"] = "Bearer " + token;
    }

    return $.ajax(options).then(
        function (response) {

            return response;

        },
        function (xhr) {

            if (xhr.status !== 401) {
                return $.Deferred()
                    .reject(xhr)
                    .promise();
            }

            return $.ajax({
                url: "/api/AuthApi/refresh",
                type: "POST"
            }).then(
                function (response) {

                    sessionStorage.setItem(
                        "jwtToken",
                        response.accessToken
                    );

                    options.headers["Authorization"] =
                        "Bearer " + response.accessToken;

                    return $.ajax(options);

                },
                function () {

                    sessionStorage.removeItem("jwtToken");

                    window.location.href =
                        "/Account/Login";

                    return $.Deferred()
                        .reject()
                        .promise();
                }
            );
        }
    );
}

function getCookie(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length === 2) return parts.pop().split(";").shift();
    return null;
}