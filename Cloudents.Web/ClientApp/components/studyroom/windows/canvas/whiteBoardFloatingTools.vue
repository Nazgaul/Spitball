<template>
   <v-btn-toggle class="whiteBoardFloatingTools" v-model="toggle_exclusive" rounded>
      
      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on" sel="pen_draw" :class="{'active-tool': selectedOptionString === enumOptions.pan}"
                        class="nav-action" @click="setOptionType($event, enumOptions.pan)">
                  <v-icon :color="selectedColor(enumOptions.pan)">sbf-pan</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_pan</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.select}"
                        class="nav-action" @click="setOptionType($event, enumOptions.select)">
                  <v-icon :color="selectedColor(enumOptions.select)">sbf-mouse-pointer</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_select</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on" sel="text_draw"
                        :class="{'active-tool': selectedOptionString === enumOptions.text, 'mouse-text': selectedOptionString === enumOptions.text}"
                        class="nav-action" @click="setOptionType($event, enumOptions.text)">
                  <v-icon :color="selectedColor(enumOptions.text)">sbf-text-icon</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_text</span>
      </v-tooltip>
      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on"
                        :class="{'active-tool': selectedOptionString === enumOptions.equation}"
                        class="nav-action" @click="setOptionType($event, enumOptions.equation)">
                  <v-icon :color="selectedColor(enumOptions.equation)">sbf-equation-icon</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_equation</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on"
                        :class="{'active-tool': selectedOptionString === enumOptions.iink}"
                        class="nav-action" @click="setOptionType($event, enumOptions.iink)">
                  <v-icon :color="selectedColor(enumOptions.iink)">sbf-fx-icon</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_iink</span>
      </v-tooltip>
            
      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon sel="color_picker" v-on="on" :class="{'active-tool': showPickColorInterface}" class="nav-action"
                        @click="showColorPicker">
                  <v-icon class="selected-color" :style="{ color: canvasData.color.hex}">sbf-color-picked</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_color</span>
      </v-tooltip>
         <slider-picker class="color-picker" 
                        :palette="predefinedColors" 
                        v-show="showPickColorInterface"
                        v-model="canvasData.color"/>
      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.draw}"
                        class="nav-action" @click="setOptionType($event, enumOptions.draw)">
                  <v-icon :color="selectedColor(enumOptions.draw)">sbf-pencil-empty</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_pen</span>
      </v-tooltip>


      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon sel="line_draw" v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.line}"
                        class="nav-action" @click="setOptionType($event, enumOptions.line)">
                  <v-icon :color="selectedColor(enumOptions.line)">sbf-connect-line</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_line</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon sel="circle_draw" v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.circle}"
                        class="nav-action" @click="setOptionType($event, enumOptions.circle)">
                  <v-icon :color="selectedColor(enumOptions.circle)">sbf-elipse-stroke</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_circle</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon sel="square_draw" v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.rectangle}"
                        class="nav-action" @click="setOptionType($event, enumOptions.rectangle)">
                  <v-icon :color="selectedColor(enumOptions.rectangle)">sbf-rectangle-stroke</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_square</span>
      </v-tooltip>

      <input class="nav-action" type="file" name="Image Upload" id="imageUpload" accept="image/*" v-show="false"/>
                
            
      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.image}"
                        class="nav-action" @click="setOptionType($event, enumOptions.image)">
                  <v-icon :color="selectedColor(enumOptions.image)">sbf-upload</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_upload</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon sel="clear_all_canvas" v-on="on" class="nav-action" @click="takeSnapshot()">
                  <v-icon style="margin-bottom: 2px;">sbf-capture-icon</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_takeSnapshot</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.eraser}"
                        class="nav-action" @click="setOptionType($event, enumOptions.eraser)">
                  <v-icon :color="selectedColor(enumOptions.eraser)" >sbf-eraser-empty</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_eraser</span>
      </v-tooltip>
      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon sel="undo_draw" v-on="on" class="nav-action" :class="{'disabled': dragData.length === 0}" @click="undo()">
                  <v-icon>sbf-undo</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_undo</span>
      </v-tooltip>

      <v-tooltip bottom>
            <template v-slot:activator="{on}">
               <v-btn icon sel="clear_all_canvas" v-on="on" class="nav-action" @click="clearCanvas()">
                  <v-icon style="margin-top: 5px;">sbf-clearAll-icon</v-icon>
               </v-btn>
            </template>
            <span v-language:inner>tutor_tooltip_clearAll</span>
      </v-tooltip>
   </v-btn-toggle>
</template>

<script>
import { mapGetters, mapActions } from 'vuex'
import helperUtil from '../../whiteboard/utils/helper.js';
import whiteBoardService from '../../whiteboard/whiteBoardService.js';

import {Compact} from 'vue-color';
// eslint-disable-next-line no-unused-vars
import textDraw from '../../whiteboard/options/textDraw.js';
// eslint-disable-next-line no-unused-vars
import equationDraw from '../../whiteboard/options/equationDraw.js';
export default {
   components:{
      'sliderPicker': Compact,
   },
   data() {
      return {
         toggle_exclusive: undefined,
         enumOptions: {
               draw: 'liveDraw',
               line: 'lineDraw',
               circle: 'drawEllipse',
               rectangle: 'drawRectangle',
               image: 'imageDraw',
               eraser: 'eraser',
               text: 'textDraw',
               equation: 'equationDraw',
               select: 'selectShape',
               pan: 'panTool',
               iink: 'iink'
         },
         predefinedColors:[
               '#000000',
               '#FF0000',
               '#00ff00',
               '#40e0d0',
               '#800000',
               '#0000ff',
               '#008000',
               '#ffd700',
               '#8a2be2',
               '#ff00ff',
               '#c0c0c0',
               '#ffff00',
               '#088da5',
               '#003366'
         ],
      }
   },
   computed:{
      ...mapGetters(['showPickColorInterface', 'selectedOptionString', 'canvasDataStore', 'getDragData']),
      dragData(){
         return this.getDragData;
      },
      canvasData(){
         return this.canvasDataStore;
      },
   },
   watch: {
      selectedOptionString(val){
         if(val === this.enumOptions.text){
               document.body.style.cursor = "text"
         }else{
               document.body.style.cursor = ""
         }
      }
   },
   methods:{
      ...mapActions(['setShowPickColorInterface', 'setCurrentOptionSelected', 'setSelectedOptionString', 'setUndoClicked', 'setClearAllClicked', 'updateDialogSnapshot']),
      selectedColor(option){
         return this.selectedOptionString === option? '#4c59ff':''
      },
      showColorPicker() {
         this.$ga.event("tutoringRoom", "showColorPicker");
         this.setShowPickColorInterface(true);
      },
      hideColorPicker() {
         this.setShowPickColorInterface(false);
      },
      selectDefaultTool(){
         this.setOptionType(this.enumOptions.select);
      },
      setOptionType(e,selectedOption) {
         this.$ga.event("tutoringRoom", selectedOption);

         this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, selectedOption)());
         this.setSelectedOptionString(selectedOption);
         // if(selectedOption === 'textDraw'){
         //     let mouseEvent = new MouseEvent("mousedown", {});
         //     global.canvas.dispatchEvent(mouseEvent);
         // } 
         if(selectedOption === 'equationDraw'){
               let mouseEvent = new MouseEvent("mousedown", {});
               global.canvas.dispatchEvent(mouseEvent);
         } else if(selectedOption === 'iink'){
               let mouseEvent = new MouseEvent("mousedown", {});
               global.canvas.dispatchEvent(mouseEvent);
         } else{
               helperUtil.HelperObj.isActive = false;
         }
         

         if(selectedOption === this.enumOptions.image){
               let inputImgElm = document.getElementById('imageUpload');
               inputImgElm.click();
               this.selectDefaultTool();
         }
         this.hideColorPicker();
      },
      undo(){
         this.$ga.event("tutoringRoom", "undo");
         this.setUndoClicked();
      },
      clearCanvas(){
         this.$ga.event("tutoringRoom", "clearCanvas");
         this.setClearAllClicked();
      },
      takeSnapshot(){
         this.$ga.event("tutoringRoom", "takeSnapshot");
         this.updateDialogSnapshot(true);
      }
   },
}
</script>

<style lang="less">
.whiteBoardFloatingTools{
   position: absolute;
   left: ~"calc(100% / 3)";
   margin-top: 14px;
   box-shadow: 0px 2px 4px -1px rgba(0, 0, 0, 0.2), 0px 4px 5px 0px rgba(0, 0, 0, 0.14), 0px 1px 10px 0px rgba(0, 0, 0, 0.1);
   .nav-action{
      &.active-tool{
         .v-icon {
            color:#4c59ff;
         }
      }
   }
}
</style>