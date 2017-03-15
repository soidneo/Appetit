$(document).ready(function () {
    $("#DepartamentoID").change(function () {
        $("#CiudadID").empty();
        $("#CiudadID").append('<option value="0">[Seleccione una ciudad...]</option>');
        $.ajax({
            type: 'POST',
            url: Url,
            dataType: 'json',
            data: { DepartamentoID: $("#DepartamentoID").val() },
            success: function (data) {
                $.each(data, function (i, data) {
                    $("#CiudadID").append('<option value="'
                        + data.CiudadID + '">'
                        + data.Nombre + '</option>');
                });
            },
            error: function (ex) {
                alert('Fallo la carga de ciudades.' + ex);
            }
        });
        return false;
    })
});
