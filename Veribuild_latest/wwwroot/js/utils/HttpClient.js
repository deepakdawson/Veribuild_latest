export default class HttpClient {
    async post(url, data, successCallback, errorCallback, element) {
        $.ajax({
            url: url,
            data: data,
            type: 'POST',
            processData: false,
            contentType: false,
            async: true,
            success: function (data, textStatus, xhr) {
                if (xhr.status == 200) {
                    successCallback(data, element);
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log(xhr);
                errorCallback(element);
            }
        });
    };

    async postJson(url, data, successCallback, errorCallback, element) {
        $.ajax({
            url: url,
            data: data,
            type: 'POST',
            contentType: 'application/json',
            async: true,
            success: function (data, textStatus, xhr) {
                if (xhr.status == 200) {
                    successCallback(data, element);
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log(xhr);
                errorCallback(element);
            }
        });
    };

    async getJson(url, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: 'GET',
            async: true,
            success: function (data, textStatus, xhr) {
                if (xhr.status == 200) {
                    successCallback(data);
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log(xhr);
                errorCallback(xhr);
            }
        });
    };
    async deleteRequest(url, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: 'DELETE',
            async: true,
            success: function (data, textStatus, xhr) {
                if (xhr.status == 200) {
                    successCallback(data);
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log(xhr);
                errorCallback(xhr);
            }
        });
    };
}