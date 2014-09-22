app.directive('quizGraph',
   ['$timeout', function ($timeout) {
       return {
           scope: false,
           restrict: "A",
       
           link: function (scope, elem, attrs) {


               var context = elem[0].getContext('2d'),
                   canvasWidth = elem[0].width,
                   canvasHeight = elem[0].height,
                    minX = -3,
                    maxX = 3,
                    minY = -0.6,
                    maxY = 0,
                    rangeX = maxX - minX,
                    rangeY = maxY - minY,
                    centerY = Math.round(Math.abs(minY / rangeY) * canvasHeight - 25),
                    centerX = Math.round(Math.abs(minX / rangeX) * canvasWidth),
                    iteration = (maxX - minX) / canvasWidth,
                    scaleX = canvasWidth / rangeX,
                    scaleY = canvasHeight / rangeY;




                  drawGrid();
                  drawGraph();





               function drawGrid() {
                   context.beginPath();
                   context.moveTo(2, 0);

                   context.lineTo(2, canvasHeight - 25);
                   context.lineTo(canvasWidth, canvasHeight - 25);                   
                   context.strokeStyle = '#797979';
                   context.lineWidth = 1;
                   context.stroke();
               }


               function drawGraph() {


                   drawEquation(function (x) {
                       return (1 / Math.sqrt(2 * Math.PI)) * Math.exp(-(Math.pow(x, 2) / 2));
                   }, 'green', 2);

                   function drawEquation(equation, color) {
                       context.save();
                       transformContext();

                       context.beginPath();
                       context.moveTo(minX, equation(minX));

                       for (var x = minX + iteration; x <= maxX; x += iteration) {
                           context.lineTo(x, equation(x));
                       }

                       context.restore();
                       context.lineJoin = 'round';
                       context.lineWidth = 2;
                       context.strokeStyle = color;
                       context.stroke();
                   };

               }


               function drawData() {
                 

                   var userAverage = scope.quiz.stdevp ? 
                                        (scope.quiz.result - scope.quiz.average) / scope.quiz.stdevp : 0,
                       averageHeight = (1 / Math.sqrt(2 * Math.PI)) * Math.exp(-(Math.pow(userAverage, 2) / 2));

                   drawFlag();
                   drawScore();
                   drawAverage();

                   function drawFlag() {
                       context.save();
                       transformContext();
                       context.beginPath();
                       context.moveTo(userAverage, 0);
                       context.lineTo(userAverage, averageHeight);
                       context.fillStyle = '#e6ad20';
                       context.fillRect(userAverage - 0.03, averageHeight, 0.8, 0.1);
                       context.closePath();
                       context.restore();
                       context.strokeStyle = '#e6ad20';
                       context.lineWidth = 3;
                       context.setLineDash([10,5]);
                       context.stroke();
                       context.setLineDash([]);

                   }

                   function drawScore() {
                       context.save();
                       context.fillStyle = 'white';
                       context.translate(centerX, centerY);
                       context.font = '12pt arial';
                       context.textBaseline = 'middle';
                       context.textAlign = 'center';
                       context.fillText(scope.quiz.result, userAverage * scaleX + 17, -averageHeight * scaleY - 9.5); //23 half flag width, 15.5 half flag height
                       context.restore();
                   }

                   function drawAverage() {
                       context.textAlign = 'center';
                       context.textBaseline = 'bottom';
                       context.fillStyle = '#e6ad20';
                       context.font = '12pt arial';
                       context.fillText('average: ' + scope.quiz.average, canvasWidth / 2, canvasHeight);
                   }
              }                                      

               attrs.$observe('userDone', function () {
                   if (attrs.userDone === 'true') {
                       drawData();
                   } else {
                       clearCanvas(); drawGrid(); drawGraph();
                   }
               });


               function clearCanvas () {
                   context.clearRect(0, 0, canvasWidth, canvasHeight);
               };

           

               function transformContext() {
                   // move context to center of canvas
                   context.translate(centerX, centerY);

                   /*
                    * stretch grid to fit the canvas window, and
                    * invert the y scale so that that increments
                    * as you move upwards
                    */
                   context.scale(scaleX, -scaleY);
               };

           }
       };

   }
   ]);
