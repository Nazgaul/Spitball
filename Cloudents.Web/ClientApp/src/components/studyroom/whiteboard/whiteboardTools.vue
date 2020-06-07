<template>
    <div class="nav-container">
            <!--Select-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" sel="pen_draw" :class="{'active-tool': selectedOptionString === enumOptions.pan}"
                            class="nav-action" @click="setOptionType($event, enumOptions.pan)">
                        <v-icon>sbf-pan</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_pan</span>
            </v-tooltip>

            <!--Select-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.select}"
                            class="nav-action" @click="setOptionType($event, enumOptions.select)">
                        <v-icon>sbf-mouse-pointer</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_select</span>
            </v-tooltip>

            <!--Text-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" sel="text_draw"
                             :class="{'active-tool': selectedOptionString === enumOptions.text, 'mouse-text': selectedOptionString === enumOptions.text}"
                             class="nav-action" @click="setOptionType($event, enumOptions.text)">
                        <v-icon>sbf-text-icon</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_text</span>
            </v-tooltip>
            <!--Equation-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button  v-on="on"
                             :class="{'active-tool': selectedOptionString === enumOptions.equation}"
                             class="nav-action" @click="setOptionType($event, enumOptions.equation)">
                        <v-icon>sbf-equation-icon</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_equation</span>
            </v-tooltip>
            <!--iink Draw-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button  v-on="on"
                             :class="{'active-tool': selectedOptionString === enumOptions.iink}"
                             class="nav-action" @click="setOptionType($event, enumOptions.iink)">
                        <v-icon>sbf-fx-icon</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_iink</span>
            </v-tooltip>
            <!--Color Picker-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button  sel="color_picker" v-on="on" :class="{'active-tool': showPickColorInterface}" class="nav-action"
                            @click="showColorPicker">
                        <v-icon class="selected-color" :style="{ color: canvasData.color.hex}">sbf-color-picked</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_color</span>
            </v-tooltip>
            <slider-picker class="color-picker" :palette="predefinedColors" v-show="showPickColorInterface"
                           v-model="canvasData.color"/>
            <!--<button class="nav-action" @click="clearCanvas">clear</button>-->

            <!--Draw-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.draw}"
                            class="nav-action" @click="setOptionType($event, enumOptions.draw)">
                        <v-icon>sbf-pencil-empty</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_pen</span>
            </v-tooltip>

            <!--Line-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button sel="line_draw" v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.line}"
                            class="nav-action" @click="setOptionType($event, enumOptions.line)">
                        <v-icon>sbf-connect-line</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_line</span>
            </v-tooltip>

            <!--Circle-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button sel="circle_draw" v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.circle}"
                            class="nav-action" @click="setOptionType($event, enumOptions.circle)">
                        <v-icon>sbf-elipse-stroke</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_circle</span>
            </v-tooltip>

            <!--Square-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button sel="square_draw" v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.rectangle}"
                            class="nav-action" @click="setOptionType($event, enumOptions.rectangle)">
                        <v-icon>sbf-rectangle-stroke</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_square</span>
            </v-tooltip>

            <!--Upload Image-->
            <input class="nav-action" type="file" name="Image Upload" id="imageUpload" accept="image/*" v-show="false"/>
                
            
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.image}"
                             class="nav-action" @click="setOptionType($event, enumOptions.image)">
                        <v-icon>sbf-upload</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_upload</span>
            </v-tooltip>

            <!--snapshot-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button sel="clear_all_canvas" v-on="on" class="nav-action" @click="takeSnapshot()">
                        <v-icon style="margin-bottom: 2px;">sbf-capture-icon</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_takeSnapshot</span>
            </v-tooltip>

            <!--eraser-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.eraser}"
                             class="nav-action" @click="setOptionType($event, enumOptions.eraser)">
                        <v-icon>sbf-eraser-empty</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_eraser</span>
            </v-tooltip>

            <!--Undo-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button sel="undo_draw" v-on="on" class="nav-action" :class="{'disabled': dragData.length === 0}" @click="undo()">
                        <v-icon>sbf-undo</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_undo</span>
            </v-tooltip>

            <!--Clear All-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button sel="clear_all_canvas" v-on="on" class="nav-action" @click="clearCanvas()">
                        <v-icon style="margin-top: 5px;">sbf-clearAll-icon</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_clearAll</span>
            </v-tooltip>
        </div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import helperUtil from './utils/helper';
import whiteBoardService from './whiteBoardService';
import {Compact} from 'vue-color';
// eslint-disable-next-line no-unused-vars
import textDraw from './options/textDraw';
// eslint-disable-next-line no-unused-vars
import equationDraw from './options/equationDraw';

export default {
    components:{
        'sliderPicker': Compact,
    },
    data(){
        return{
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
    methods:{
        ...mapActions(['setShowPickColorInterface', 'setCurrentOptionSelected', 'setSelectedOptionString', 'setUndoClicked', 'setClearAllClicked', 'updateDialogSnapshot']),
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
    computed:{
        ...mapGetters(['showPickColorInterface', 'selectedOptionString', 'canvasDataStore', 'getDragData']),
        dragData(){
            return this.getDragData;
        },
        canvasData(){
            return this.canvasDataStore;
        }
    },
    watch:{
        selectedOptionString(val){
            if(val === this.enumOptions.text){
                document.body.style.cursor = "text"
            }else{
                document.body.style.cursor = ""
            }
        }
    }

}
</script>

<style lang="less">
.nav-container {
            /*position: fixed;*/
            background-color: #FFFFFF;
            /*padding: 0px 0;*/
            display: flex;
            flex-direction: row;
            width: auto;
            /*margin-top: 20px;*/
            /*left: 16px;*/
            z-index: 5;

            &.bottom-nav{
                bottom: 16px;
            }
            .nav-action {
                padding: 0 10px;
                outline: none!important;
                .v-icon {
                    color: #000000;
                }
                &.disabled{
                    pointer-events: none;
                    .v-icon {
                        color: rgba(0,0,0,.3);
                    }
                }
                &.active-tool{
                    .v-icon {
                        color:#2a79ff;
                    }
                    background:#cfe1ff;
                }
                &.active-tool-svg{
                    svg {
                        fill:#2a79ff;
                    }
                }
                &:hover {
                    .v-icon {
                        color: #2a79ff;
                    }

                }
            }
            .selected-color {
                font-size: 26px;
                cursor: pointer;
            }
            .color-picker {
                position: absolute;
                top: 48px;
                left: 220px;
                height: auto;
                .vc-compact-color-item {
                    list-style: none;
                    width: 28px;
                    height: 28px;
                    border-radius: 28px;
                    float: left;
                    margin-right: 5px;
                    margin-bottom: 5px;
                    position: relative;
                    cursor: pointer;
                }
            }

        }
</style>
