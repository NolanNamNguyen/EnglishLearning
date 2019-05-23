
$(function () {
    $("#PuzzleActive").click(function () {
        $(".PuzzleGamePick").removeClass("inviImage");
        $(".IncomingGamespick").addClass("inviImage");
        $(".QuizPick").addClass("inviImage");
    });
    $("#IncomingActive").click(function () {
        $(".PuzzleGamePick").addClass("inviImage");
        $(".IncomingGamespick").removeClass("inviImage");
        $(".QuizPick").addClass("inviImage");
    });
    $("#QuizActive").click(function () {
        $(".PuzzleGamePick").addClass("inviImage");
        $(".IncomingGamespick").addClass("inviImage");
        $(".QuizPick").removeClass("inviImage");
    });
})
