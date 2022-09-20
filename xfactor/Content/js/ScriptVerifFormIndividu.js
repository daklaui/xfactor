//verifier form de persone morale
$("#myForm").validate({
    rules: {
        // compound rule
        FROM_JUR_IND: {
            required: true
        },
        NOM_IND: {
            required: true,
        },
        PRE_IND: {
            required: true,
        },
        NUM_DOC_ID_IND: {
            required: true,
        },
        LIEU_DOC_ID_IND: {
            required: true,
        },
        COD_SCLAS: {
            required: true,
        },
        DATE_DOC_ID_IND: {
            required: true,
        }
,
        COD_TVA_IND: {
            required: true,
            maxlength: 8
        },
        untextici: {
            required: true
        }
        , REF_ACH_IND: { required: true, number: true, min: 1 },
        REF_ADH_IND: { required: true, number: true, min: 1 }



    },

    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error');
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').removeClass('has-error').addClass('has-success');
    }


});

$.validator.addClassRules("verifeRib1", {
    number: true,
    minlength: 2,
    maxlength: 2
});
$.validator.addClassRules("verifeRib2", {
    number: true,
    minlength: 3,
    maxlength: 3
});
$.validator.addClassRules("verifeRib3", {
    number: true,
    minlength: 15,
    maxlength: 15
});


//verifier form de persone phy
$("#myFormPP").validate({
    rules: {
        // compound rule

        NOM_IND: {
            required: true
        },
        PRE_IND: {
            required: true
        },
        NUM_DOC_ID_IND: {
            required: true
        },
        LIEU_DOC_ID_IND: {
            required: true
        },
        DAT_NAISS_CREAT: {
            required: true
        },
        DATE_DOC_ID_IND: {
            required: true
        },
        REF_ACH_IND: { required: true, number: true, min: 1 },
        REF_ADH_IND: { required: true, number: true, min: 1 }
         ,
        untextici: {
            required: true
        }


    },

    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error');
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').removeClass('has-error').addClass('has-success');
    }


});


//verifier form de contact
var formcontact = $("#formcontact");
formcontact.validate({
    rules: {
        NOM_PRE_CONTACT: {
            required: true
        },
        POS_CONTACT: {
            required: true
        },
        MAIL1_COONTACT: {
            required: true
        },
        FAX_CONTACT: {
            required: true
        }
    },
    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error');
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').removeClass('has-error').addClass('has-success');
    }


});