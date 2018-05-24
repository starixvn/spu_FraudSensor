$(function () {
    $("#fraud-case-activator").change(function () {
        var $this = $(this);

        var url = common.modifyRelativeUrl("/FraudCase/ChangeCaseStatus");

        console.log("adfadf");

        $.post(url, { fraudCaseId: $this.attr("data-id"), isActive: $this.prop("checked") }, () => common.showSuccess());
    });

    $("select").select2({
        minimumResultsForSearch: -1,
        width: "300px",
        dropdownAutoWidth: true
    });
})