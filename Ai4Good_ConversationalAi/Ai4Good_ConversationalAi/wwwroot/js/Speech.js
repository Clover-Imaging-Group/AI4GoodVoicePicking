function PlayBase64Audio(data) {
    $("#voiceAudio").attr("src", `data:audio/wav;base64,${data}`);
};