var keepAlive = {
    refresh: function () {
        $.get('/keep-alive.ashx');
        setTimeout(keepAlive.refresh, 6000000);
    }
}; $(document).ready(keepAlive.refresh());