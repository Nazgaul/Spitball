<template>
    <div class="canvas-container" :style="`max-width:${canvasWidth}px;`" id="canvasDiv">
        <div class="powered-by">
            <div>
                <h4 class="text-sm-center text-md-center">Powered by</h4>
                <AppLogo></AppLogo>
            </div>
            <div>
                <share-room-btn v-show="isRoomCreated"></share-room-btn>
            </div>
        </div>
        <div class="nav-container elevation-2">
            <!--Select-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="[selectedOptionString === enumOptions.select ? 'active-tool' : '']"
                            class="nav-action" @click="setOptionType(enumOptions.select)">
                        <v-icon>sbf-mouse-pointer</v-icon>
                    </button>
                </template>
                <span >Select</span>
            </v-tooltip>

            <!--Text-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button  v-on="on"
                             :class="[selectedOptionString === enumOptions.text ? 'active-tool' : '']"
                             class="nav-action" @click="setOptionType(enumOptions.text)">
                        <v-icon>sbf-text-icon</v-icon>
                    </button>
                </template>
                <span >Text</span>
            </v-tooltip>
            <!--Color Picker-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="[showPickColorInterface ? 'active-tool' : '']" class="nav-action"
                            @click="showColorPicker">
                        <span class="selected-color" :style="{ backgroundColor: canvasData.color.hex}"></span>
                    </button>
                </template>
                <span >Color Picker</span>
            </v-tooltip>
            <slider-picker class="color-picker" :palette="predefinedColors" v-show="showPickColorInterface"
                           v-model="canvasData.color"/>
            <!--<button class="nav-action" @click="clearCanvas">clear</button>-->

            <!--Draw-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="[selectedOptionString === enumOptions.draw ? 'active-tool' : '']"
                            class="nav-action" @click="setOptionType(enumOptions.draw)">
                        <v-icon>sbf-pencil-empty</v-icon>
                    </button>
                </template>
                <span >Pen</span>
            </v-tooltip>

            <!--Line-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="[selectedOptionString === enumOptions.line ? 'active-tool' : '']"
                            class="nav-action" @click="setOptionType(enumOptions.line)">
                        <v-icon>sbf-connect-line</v-icon>
                    </button>
                </template>
                <span >Line</span>
            </v-tooltip>

            <!--Circle-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="[selectedOptionString === enumOptions.circle ? 'active-tool' : '']"
                            class="nav-action" @click="setOptionType(enumOptions.circle)">
                        <v-icon>sbf-eclipse</v-icon>
                    </button>
                </template>
                <span>Circle</span>
            </v-tooltip>

            <!--Square-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="[selectedOptionString === enumOptions.rectangle ? 'active-tool' : '']"
                            class="nav-action" @click="setOptionType(enumOptions.rectangle)">
                        <v-icon>sbf-square</v-icon>
                    </button>
                </template>
                <span>Square</span>
            </v-tooltip>

            <!--Upload Image-->
            <input class="nav-action" type="file" name="Image Upload" id="imageUpload" v-show="false"/>
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" :class="[selectedOptionString === enumOptions.image ? 'active-tool' : '']"
                             class="nav-action" @click="setOptionType(enumOptions.image)">
                        <v-icon>sbf-upload</v-icon>
                    </button>
                </template>
                <span>Upload Image</span>
            </v-tooltip>

            <!--<button :class="[selectedOptionString === enumOptions.eraser ? 'active-tool' : '']"-->
                    <!--class="nav-action" @click="setOptionType(enumOptions.eraser)">-->
                <!--<v-icon>sbf-eraser-empty</v-icon>-->
            <!--</button>-->

            <!--Undo-->
            <v-tooltip right>
                <template v-slot:activator="{on}">
                    <button v-on="on" class="nav-action" @click="undo();">
                        <v-icon>sbf-undo</v-icon>
                    </button>
                </template>
                <span>Undo</span>
            </v-tooltip>

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
                  :style="{'stroke': helperStyle.stroke}"/>

            <line v-if="selectedOptionString === enumOptions.line"
                  :x1="helperStyle.x1"
                  :y1="helperStyle.y1"
                  :x2="helperStyle.x2"
                  :y2="helperStyle.y2"
                  :class="helperClass"
                  :style="{'stroke': helperStyle.stroke}"/>

            <ellipse v-if="selectedOptionString === enumOptions.circle"
                     :cx="helperStyle.cx"
                     :cy="helperStyle.cy"
                     :rx="helperStyle.rx"
                     :ry="helperStyle.ry"
                     :class="helperClass"
                     :style="{'stroke': helperStyle.stroke}"/>
        </svg>
        <div class="text-helper-container" v-if="helperShow">
            <input type="text" placeholder="Enter Some Text"
                   v-if="selectedOptionString === enumOptions.text"
                   v-model="helperStyle.text"
                   :class="[helperClass, helperStyle.id]"
                   :style="{'color': helperStyle.color, 'top':helperStyle.top, 'left':helperStyle.left}"/>
        </div>
        <!--<div>-->
            <!--<ul>-->
                <!--<li v-for="(shape, index) in canvasData.dragData" :key="index">{{shape.type}}</li>-->
            <!--</ul>-->
        <!--</div>-->
    </div>
</template>

<script src="./whiteBoard.js"></script>


<style lang="less">
    .canvas-container {
        display: flex;
        flex-direction: column;
        margin: 0 auto;
        .powered-by {
            display: inline-flex;
            max-width: 350px;
            position: fixed;
            top: 25px;
            .logo {
                margin-right: 40px;
                fill: #3e45a0;
            }
        }
        .nav-container {
            position: fixed;
            background-color: #FFFFFF;
            padding: 16px 0;
            display: flex;
            flex-direction: column;
            width: auto;
            margin-top: 150px;
            .nav-action {
                padding: 12px 16px;
                outline: none!important;
                .v-icon {
                    color: #000000;
                }
                &.active-tool{
                    .v-icon {
                        color:#2a79ff;
                    }
                }
                &:hover {
                    .v-icon {
                        color: #2a79ff;
                    }

                }
            }
            .selected-color {
                display: inline-flex;
                width: 28px;
                height: 28px;
                border-radius: 28px;
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
        canvas {
            &.select-object {
                cursor: pointer;
            }
        }
        .helper {
            position: absolute;
            pointer-events: none;
            top: 0;
            left: 0;
            .rectangular-helper {
                pointer-events: none;
                position: absolute;
                stroke-dasharray: 5, 5;
                fill: rgba(173, 201, 185, 0.05);
                stroke-width: 1px;
            }
            .line-helper {
                pointer-events: none;
                position: absolute;
                stroke-dasharray: 5, 5;
                fill: rgba(173, 201, 185, 0.05);
                stroke-width: 1px;
            }
            .ellipse-helper {
                pointer-events: none;
                position: absolute;
                stroke-dasharray: 5, 5;
                fill: rgba(173, 201, 185, 0.05);
                stroke-width: 1px;
            }
        }
        .text-helper-container {
            .text-helper {
                position: absolute;
                border: none;
                background-color: rgba(0, 0, 0, 0.05);
                outline: none;
                border-radius: 4px;
                font-family: sans-serif;
                font-size: 14px;
            }
        }
    }
</style>
