$(function () {
    $("select").select2({
        minimumResultsForSearch: -1,
        width: "300px",
        dropdownAutoWidth: true
    });

    if (!isHost) {
        $(".-primary:not(#search-btn)").prop("disabled", true);
        $(document).ajaxSuccess(() => $(".icon-delete").unbind("click").css("color", "gray"));
    }
    else {
        $("#fraud-case-activator").change(function () {
            var $this = $(this);

            var url = common.modifyRelativeUrl("/FraudCase/ChangeCaseStatus");

            $.post(url, { fraudCaseId: $this.attr("data-id"), isActive: $this.prop("checked") }, () => common.showSuccess());
        });
    }
})