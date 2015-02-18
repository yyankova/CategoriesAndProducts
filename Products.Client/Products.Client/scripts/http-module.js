var httpModule = (function httpModule($) {
    function makeRequest(url, type, data, onSuccess, onError) {
        $.ajax(url, {
            type: type,
            data: data,
            success: onSuccess,
            error: onError
            });
    }
    
    function get(url, onSuccess, onError) {
        makeRequest(url, 'GET', null, onSuccess, onError);
    }

    function post(url, data, onSuccess, onError) {
        makeRequest(url, 'POST', data, onSuccess, onError);
    }

    function put(url, data, onSuccess, onError) {
        makeRequest(url, 'PUT', data, onSuccess, onError);
    }

    function del(url, data, onSuccess, onError) {
        makeRequest(url, 'DELETE', data, onSuccess, onError);
    }

    return {
        get: get,
        post: post,
        put: put,
        del: del
    }
}(jQuery));