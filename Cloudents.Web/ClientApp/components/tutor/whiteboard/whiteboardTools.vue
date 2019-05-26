<template>
    <div class="nav-container elevation-2">
            <!--Select-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.pan}"
                            class="nav-action" @click="setOptionType(enumOptions.pan)">
                        <v-icon>sbf-pan</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_pan</span>
            </v-tooltip>

            <!--Select-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.select}"
                            class="nav-action" @click="setOptionType(enumOptions.select)">
                        <v-icon>sbf-mouse-pointer</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_select</span>
            </v-tooltip>

            <!--Text-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button  v-on="on"
                             :class="{'active-tool': selectedOptionString === enumOptions.text}"
                             class="nav-action" @click="setOptionType(enumOptions.text)">
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
                             class="nav-action" @click="setOptionType(enumOptions.equation)">
                        <v-icon>sbf-equation-icon</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_equation</span>
            </v-tooltip>
            <!--Color Picker-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': showPickColorInterface}" class="nav-action"
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
                            class="nav-action" @click="setOptionType(enumOptions.draw)">
                        <v-icon>sbf-pencil-empty</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_pen</span>
            </v-tooltip>

            <!--Line-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.line}"
                            class="nav-action" @click="setOptionType(enumOptions.line)">
                        <v-icon>sbf-connect-line</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_line</span>
            </v-tooltip>

            <!--Circle-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.circle}"
                            class="nav-action" @click="setOptionType(enumOptions.circle)">
                        <v-icon>sbf-elipse-stroke</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_circle</span>
            </v-tooltip>

            <!--Square-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.rectangle}"
                            class="nav-action" @click="setOptionType(enumOptions.rectangle)">
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
                             class="nav-action" @click="setOptionType(enumOptions.image)">
                        <v-icon>sbf-upload</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_upload</span>
            </v-tooltip>

            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="{'active-tool': selectedOptionString === enumOptions.eraser}"
                             class="nav-action" @click="setOptionType(enumOptions.eraser)">
                        <v-icon>sbf-eraser-empty</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_upload</span>
            </v-tooltip>

            <!--<button :class="[selectedOptionString === enumOptions.eraser ? 'active-tool' : '']"-->
                    <!--class="nav-action" @click="setOptionType(enumOptions.eraser)">-->
                <!--<v-icon>sbf-eraser-empty</v-icon>-->
            <!--</button>-->

            <!--Undo-->
            <v-tooltip bottom>
                <template v-slot:activator="{on}">
                    <button v-on="on" class="nav-action" :class="{'disabled': dragData.length === 0}" @click="undo()">
                        <v-icon>sbf-undo</v-icon>
                    </button>
                </template>
                <span v-language:inner>tutor_tooltip_undo</span>
            </v-tooltip>

        </div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import helperUtil from './utils/helper';
import whiteBoardService from './whiteBoardService';
import {Compact} from 'vue-color';

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
        ...mapActions(['setShowPickColorInterface', 'setCurrentOptionSelected', 'setSelectedOptionString', 'setUndoClicked']),
        showColorPicker() {
            this.setShowPickColorInterface(true);
        },
        hideColorPicker() {
            this.setShowPickColorInterface(false);
        },
        selectDefaultTool(){
            this.setOptionType(this.enumOptions.select);
        },
        setOptionType(selectedOption) {
            this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, selectedOption)());
            this.setSelectedOptionString(selectedOption);
            helperUtil.HelperObj.isActive = false;
            if(selectedOption === this.enumOptions.image){
                let inputImgElm = document.getElementById('imageUpload');
                inputImgElm.click();
                this.selectDefaultTool();
            }
            this.hideColorPicker();
        },
        undo(){
            this.setUndoClicked();
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
    }
}
</script>

<style lang="less">
.nav-container {
            position: fixed;
            background-color: #FFFFFF;
            padding: 0px 0;
            display: flex;
            flex-direction: row;
            width: auto;
            margin-top: 20px;
            left: 16px;
            z-index: 5;

            &.bottom-nav{
                bottom: 16px;
            }
            .nav-action {
                padding: 12px 10px;
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
                top: 106px;
                left: 80px;
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
