//Script de view individu 
//action de click sur button de id ajouter_groupe  pour ajouter un groupe 
$("#ajouter_groupe").click(function () {
    if ($("#NOM_GROUP").val() != "") {
        $.ajax({
            url: "/Individu/AjouterGroupe/" + $("#NOM_GROUP").val(),
            type: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (data) {
                if (data == true) {
                    alert("Groupe Bien Ajouté ");
                    $("#NOM_GROUPE").empty();
                    $.ajax({
                        url: "/Individu/ListeGroupe",
                        type: "GET",
                        contentType: "application/json;charset=UTF-8",
                        dataType: "json",
                        success: function (data) {

                            $("#NOM_GROUPE").append("<option value='' disabled selected>***************</option>");
                            $.each(data, function (i, item) {

                                $("#NOM_GROUPE").append("<option value='" + item.ID_GROUP + "'>" + item.NOM_GROUP + "</option>")
                            });
                            $("#NOM_GROUPE").append("<option value='Creation' >Ajouter un groupe</option>");

                            $("#Groupe").modal("hide");
                        }
                    });
                }

            }
        });
    }
});
//action de click  sur option d ajouter un groupe
$("#NOM_GROUPE").change(function () { if ($("#NOM_GROUPE").val() == "Creation") $("#Groupe").modal(); })



//action de click sur button ajouter contact 


$("#ajoutercontact").click(function () {
    var compteur = 0;
    if ($("#tab1").hasClass("active")) {
        if (formcontact.valid()) {
            var ACTIF_CONTACT = "";
            if ($("#ACTIF_CONTACT").parent('.icheckbox_minimal-blue').hasClass("checked") == true) {
                ACTIF_CONTACT = "value='true' checked/>";
            }
            else {
                ACTIF_CONTACT = "value='false' />";
            }
            var markup = "<tr>" +
            "<td style='display:none'><input type='text' name='cont[" + compteur + "].NOM_PRE_CONTACT' value='" + $("#NOM_PRE_CONTACT").val() + "'/> </td>" +
            "<td>" + $("#NOM_PRE_CONTACT").val() + "</td>" +
            "<td><input type='text' name='cont[" + compteur + "].POS_CONTACT' value='" + $("#POS_CONTACT").val() + "'style='border:none' size='5'/></td>" +
            "<td><input type='text' name='cont[" + compteur + "].TEL1_CONTACT' value='" + $("#TEL1_CONTACT").val() + "'style='border:none' size='8'/></td>" +
            "<td><input type='text' name='cont[" + compteur + "].MAIL1_COONTACT' value='" + $("#MAIL1_COONTACT").val() + "'style='border:none' size='10'/></td>" +
            "<td><input type='text' name='cont[" + compteur + "].FAX_CONTACT' value='" + $("#FAX_CONTACT").val() + "'style='border:none' size='8'/></td>" +
            "<td><input type='checkbox' name='cont[" + compteur + "].ACTIF_CONTACT'" + ACTIF_CONTACT + " </td>" +
           " <td><button type='button'class='btn btn-primary deletebtn'>X</button></td>" +
            "</tr>";
            compteur++;
            $("#ContactPM tbody").append(markup);
            $('#myModal').modal('toggle'); //Open Moda
            $("#formcontact").trigger('reset');
        }
    }
    if ($("#tab2").hasClass("active")) {
        var compteurPP = 0;
        if (formcontact.valid()) {
            var ACTIF_CONTACT = "";
            if ($("#ACTIF_CONTACT").parent('.icheckbox_minimal-blue').hasClass("checked") == true) {
                ACTIF_CONTACT = "value='true' checked/>";
            }
            else {
                ACTIF_CONTACT = "value='false' />";
            }
            var markup = "<tr>" +
            "<td style='display:none'><input type='text' name='cont[" + compteurPP + "].NOM_PRE_CONTACT' value='" + $("#NOM_PRE_CONTACT").val() + "'/> </td>" +
            "<td>" + $("#NOM_PRE_CONTACT").val() + "</td>" +
            "<td><input type='text' name='cont[" + compteurPP + "].POS_CONTACT' value='" + $("#POS_CONTACT").val() + "' style='border:none' size='5'/></td>" +
            "<td><input type='text' name='cont[" + compteurPP + "].TEL1_CONTACT' value='" + $("#TEL1_CONTACT").val() + "' style='border:none'  size='8'/></td>" +
            "<td><input type='text' name='cont[" + compteurPP + "].MAIL1_COONTACT' value='" + $("#MAIL1_COONTACT").val() + "'style='border:none' size='10'/></td>" +
            "<td><input type='text' name='cont[" + compteurPP + "].FAX_CONTACT' value='" + $("#FAX_CONTACT").val() + "'style='border:none' size='8'/></td>" +
            "<td><input type='checkbox' name='cont[" + compteurPP + "].ACTIF_CONTACT'" + ACTIF_CONTACT + " </td>" +
           " <td><button type='button'class='btn btn-primary deletebtn'>X</button></td>" +
            "</tr>";
            compteurPP++;
            $("#ContactPP tbody").append(markup);
            $('#myModal').modal('toggle');
            $("#formcontact").trigger('reset');
        }
    }
});

// ajouter des rib 
var compteurRib = 1;
$("#ajouterRIB").click(function () {

    $('#myTable tbody tr:last').after('<tr>' +
            '<td width="50%"> <input type="number" class="verifeRib1" name="ribs[' + compteurRib + '].RIB_RIB1" id="code_bk" size="2" style="width:10%" />-<input type="number" class="verifeRib2"  name="ribs[' + compteurRib + '].RIB_RIB2" id="code_Ag" size="2" style="width:20%" />-<input type="number"  name="ribs[' + compteurRib + '].RIB_RIB3" class="verifeRib3"/></td>'
        + '  <td><input type="text" class="nom_bq" readonly /></td>' + '  <td><input type="text" class="nom_Ag" readonly /></td>'
        + '<td><button type="button"  class="btn btn-sm btn-danger SupprimerTRRIB">-</button></td>' +
        '</tr>');
    compteurRib++;
});
//ajouter rib pour tab2 (individu morale)
var compteurRib2 = 1;
$("#ajouterRIB2").click(function () {

    $('#myTable2 tbody tr:last').after('<tr>' +
            '<td width="50%"> <input type="number" class="verifeRib1" name="ribs[' + compteurRib + '].RIB_RIB1" id="code_bk" size="2" style="width:10%" />-<input type="number" class="verifeRib2"  name="ribs[' + compteurRib + '].RIB_RIB2" id="code_Ag" size="2" style="width:20%" />-<input type="number"  name="ribs[' + compteurRib + '].RIB_RIB3" class="verifeRib3"/></td>'
        + '  <td><input type="text" class="nom_bq" readonly /></td>' + '  <td><input type="text" class="nom_Ag" readonly /></td>'
        + '<td><button type="button"  class="btn btn-sm btn-danger SupprimerTRRIB">-</button></td>' +
        '</tr>');
    compteurRib2++;
});

$(document).on('click', 'button.deletebtn', function () {
    $(this).closest('tr').remove();
    return false;
});


$(document).on('click', 'button.SupprimerTRRIB', function () {
    $(this).closest('tr').remove();
    compteurRib--;
    return false;
});

$(document).on('click', 'button.SupprimerTRRIB', function () {
    $(this).closest('tr').remove();
    compteurRib2--;
    return false;
});