var httpModule = (function httpModule($) {
    function makeRequest(url, type, data, headers, onSuccess, onError) {
        $.ajax(url, {
            type: type,
            data: data,
            headers: headers,
            success: onSuccess,
            error: onError
            });
    }
    
    function get(url, headers, onSuccess, onError) {
        makeRequest(url, 'GET', null, headers, onSuccess, onError);
    }

    function post(url, headers, data, onSuccess, onError) {
        makeRequest(url, 'POST', data, headers, onSuccess, onError);
    }

    function put(url, headers, data, onSuccess, onError) {
        makeRequest(url, 'PUT', data, headers, onSuccess, onError);
    }

    function del(url, headers, data, onSuccess, onError) {
        //makeRequest(url, 'DELETE', data, headers, onSuccess, onError);
        //If the upper code does not work
        $.ajax(url, {
            type: 'POST',
            data: { _method: 'delete' },
            headers: headers,
            success: onSuccess,
            error: onError
        });
    }

    return {
        get: get,
        post: post,
        put: put,
        del: del
    }
}(jQuery));