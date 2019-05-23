$(function () {
    $("input[name='MultiAnswer']").click(function () {
        $(".Answerbox__MultipleChoice").removeClass("Answerbox__changecolor");
        $("input[name='MultiAnswer']").parent().parent().addClass("Answerbox__changecolor");
    })

})