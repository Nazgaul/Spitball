<template>
  <div class="canvas-container" :style="`max-width:${canvasWidth}px;`" id="canvasDiv">
      <div class="powered-by">
          <div>
              <h4 class="text-sm-center text-md-center">Powered by</h4>
              <AppLogo></AppLogo>
          </div>
         <div>
             <share-room-btn></share-room-btn>
         </div>

      </div>
      <div class="nav-container elevation-2">
        <button class="nav-action" @click="clearCanvas">clear</button>&nbsp;&nbsp;&nbsp;
        <slider-picker v-show="showPickColorInterface" v-model="canvasData.color" />
        <button class="nav-action" @click="showColorPicker">Select Color</button>&nbsp;&nbsp;&nbsp;
        <button class="nav-action" @click="setOptionType(enumOptions.draw)">Draw</button>
        <button class="nav-action" @click="setOptionType(enumOptions.line)">Line</button>
        <button class="nav-action" @click="setOptionType(enumOptions.circle)">Ellipse</button>
        <button class="nav-action" @click="setOptionType(enumOptions.rectangle)">Rectangle</button>
        <button class="nav-action" @click="setOptionType(enumOptions.image)">Upload Image</button>
        <button class="nav-action" @click="setOptionType(enumOptions.eraser)">Eraser</button>
        <button class="nav-action" @click="setOptionType(enumOptions.text)">Text</button>
        <button class="nav-action" @click="setOptionType(enumOptions.select)">Select</button>
        <input class="nav-action" type="file" name="Image Upload" id="imageUpload" v-show="false"/>
      </div>
      <canvas id="canvas" :class="{'select-object': canvasData.objDetected}"></canvas>
      <!-- <svg class="helper"  v-if="helperShow"> -->
      <svg class="helper" width="100%" height="100%" v-if="helperShow">
        <rect v-if="selectedOptionString === enumOptions.rectangle || selectedOptionString === enumOptions.select"
              :x="helperStyle.x"
              :y="helperStyle.y"
              :width="helperStyle.width"
              :height="helperStyle.height"
              :class="helperClass"
              :style="{'stroke': helperStyle.stroke}" />

        <line v-if="selectedOptionString === enumOptions.line"
              :x1="helperStyle.x1"
              :y1="helperStyle.y1"
              :x2="helperStyle.x2"
              :y2="helperStyle.y2"
              :class="helperClass"
              :style="{'stroke': helperStyle.stroke}" />

        <ellipse v-if="selectedOptionString === enumOptions.circle"
                 :cx="helperStyle.cx"
                 :cy="helperStyle.cy"
                 :rx="helperStyle.rx"
                 :ry="helperStyle.ry"
                 :class="helperClass"
                 :style="{'stroke': helperStyle.stroke}" />
      </svg>
      <div class="text-helper-container" v-if="helperShow">
          <input type="text" placeholder="Enter Some Text"
                 v-if="selectedOptionString === enumOptions.text"
                 v-model="helperStyle.text"
                 :class="[helperClass, helperStyle.id]"
                 :style="{'color': helperStyle.color, 'top':helperStyle.top, 'left':helperStyle.left}"/>
      </div>
      <div>
          <ul>
              <li v-for="(shape, index) in canvasData.dragData" :key="index">{{shape.type}}</li>
          </ul>
      </div>
  </div>
</template>

<script src="./whiteBoard.js"></script>


<style lang="less">
.canvas-container{
    display:flex;
    flex-direction: column;
    margin:0 auto;
    .powered-by{
        display: inline-flex;
        max-width: 350px;
        position: fixed;
        top: 25px;
        .logo {
            margin-right: 40px;
            fill: #3e45a0;
        }
    }

    .nav-container{
        position:fixed;
        background-color: #FFFFFF;
        padding: 16px 0;
        display: flex;
        flex-direction: column;
        width: 90px;
        margin-top: 150px;
        .nav-action{
            margin: 8px 0;
            padding: 6px;
            &:hover{
                background-color: darken(#ffffff, 20%);
            }
        }
        .vc-slider{
            position: absolute;
            top:30px;
            left:8px;
        }
    }
    canvas{
        &.select-object{
            cursor: pointer;
        }
    }
    .helper{
        position: absolute;
        pointer-events: none;
        top:0;
        left:0;
        .rectangular-helper{
            pointer-events: none;
            position: absolute;
            stroke-dasharray: 5,5;
            fill: rgba(173, 201, 185, 0.05);
            stroke-width: 1px;
        }
        .line-helper{
            pointer-events: none;
            position: absolute;
            stroke-dasharray: 5,5;
            fill: rgba(173, 201, 185, 0.05);
            stroke-width: 1px;
        }
        .ellipse-helper{
            pointer-events: none;
            position: absolute;
            stroke-dasharray: 5,5;
            fill: rgba(173, 201, 185, 0.05);
            stroke-width: 1px;
        }
    }
    .text-helper-container{
        .text-helper{
            position: absolute;
            border:none;
            background-color: rgba(0, 0, 0, 0.05);
            outline: none;
            border-radius: 4px;
            font-family: sans-serif;
            font-size: 14px;
        }
    }
}
</style>
