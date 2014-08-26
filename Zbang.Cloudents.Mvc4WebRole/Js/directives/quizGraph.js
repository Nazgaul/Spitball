app.directive('quizGraph',
   [function () {
       return {
           scope: false,
           restrict: "A",
           link: function (scope, elem, attrs) {


               var context = elem[0].getContext('2d');

               //draw grid
               context.beginPath();
               context.moveTo(0, 0);
               context.lineTo(0, elem[0].height-20);
               context.lineTo(elem[0].width, elem[0].height-20);
               context.strokeStyle = '#797979';
               context.lineWidth = 1
               context.stroke();




               //draw equation

               var minX = -3,
                   maxX = 3,
                   minY = -0.6,
                   maxY = 0,
                   rangeX = maxX - minX,
                   rangeY = maxY - minY,
                   centerY = Math.round(Math.abs(minY / rangeY) * elem[0].height-20),
                   centerX = Math.round(Math.abs(minX / rangeX) * elem[0].width),
                   iteration = (maxX - minX) / elem[0].width,
                   scaleX = elem[0].width / rangeX,
                   scaleY = elem[0].height / rangeY;

               drawEquation(function (x) {

                   return (1 / Math.sqrt(2 * Math.PI)) * Math.exp(-(Math.pow(x, 2) / 2));
               }, 'green', 2);

               

               //draw data
               context.save();
               transformContext();
               context.beginPath();

               var userscore = 0;

               var y = (1 / Math.sqrt(2 * Math.PI)) * Math.exp(-(Math.pow(userscore, 2) / 2));
               context.moveTo(userscore, 0);
               context.lineTo(userscore, y);

               context.closePath();

         

               context.restore();               
               context.strokeStyle = '#e6ad20';
               context.lineWidth = 3;
               context.setLineDash([10]);

        

               context.stroke();

               context.save();

               transformContext();

               var flagTopLeft = {
                   x: userscore,
                   y: y

               }
               context.beginPath();
               context.fillStyle = '#e6ad20';
               context.fillRect(flagTopLeft.x-0.03, flagTopLeft.y, 1, 0.1);
               context.closePath();
               context.restore();

               context.stroke();
               
        
               context.setLineDash([]);

               context.save();
               context.fillStyle = 'white';
               context.translate(centerX, centerY);
               ////context.scale(scaleX, scaleY);
               context.font = '14pt arial';
               context.textBaseline = 'middle';
               context.textAlign = 'center';


               //context.scale(0.01, 0.01);
               context.fillText("100", userscore * scaleX + 23,-y * scaleY- 15.5);
               

               context.restore();




               context.textAlign = 'center';
               context.textBaseline = 'bottom';
               context.fillStyle = '#e6ad20';
               context.font = '12pt arial';
               context.fillText('average: ' + 0, elem[0].width / 2, elem[0].height);


               //draw average score

               function drawEquation(equation, color, thickness) {
                   context.save();
                   transformContext();

                   context.beginPath();
                   context.moveTo(minX, equation(minX));

                   for (var x = minX + iteration; x <= maxX; x += iteration) {
                       context.lineTo(x, equation(x));
                   }

                   context.restore();
                   context.lineJoin = 'round';
                   context.lineWidth = 3;
                   context.strokeStyle = color;
                   context.stroke();
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
