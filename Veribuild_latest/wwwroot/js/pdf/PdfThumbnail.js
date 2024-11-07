export default class PdfThumbnail {
    create(file) {
        if (pdfjsLib === undefined || pdfjsLib == null) {
            throw new Error("pdfjs library not included");
        }
        pdfjsLib.GlobalWorkerOptions.workerSrc = '/lib/pdfjs/build/pdf.worker.js';
        let fr = new FileReader();
        fr.onload = e => {
            try {
                pdfjsLib.getDocument(new Uint8Array(e.target.result)).promise.then(p => {
                    p.getPage(1).then(page => {
                        var viewport = page.getViewport({ scale: 1, });
                        // Support HiDPI-screens.
                        var outputScale = 256 / viewport.width;
                        var width = Math.floor(viewport.width * outputScale);
                        var height = Math.floor(viewport.height * outputScale);

                        const canvas = document.createElement('canvas');
                        canvas.style.width = canvas.width = width;

                        var transform = outputScale !== 1
                            ? [outputScale, 0, 0, outputScale, 0, 0]
                            : null;

                        var renderContext = {
                            canvasContext: canvas.getContext('2d'),
                            transform: transform,
                            viewport: viewport
                        };

                        page.render(renderContext).promise.then(function () {
                            return new Promise(() => {
                                return canvas.toDataURL();
                            }, () => {

                            });
                        });
                    });
                });
            } catch (error) {
                console.error(error);
            }
        };
        fr.readAsArrayBuffer(file);
    }
}