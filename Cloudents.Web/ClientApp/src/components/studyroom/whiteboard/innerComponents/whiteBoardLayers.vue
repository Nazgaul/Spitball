<template>
  <div class="whiteBoard_layers_container">
    <div class="whiteBoard_layers_layer_title">
        {{tabName}} Layers:
    </div>
    <div class="whiteBoard_layers_layer_item" v-for="(shape, index) in layers" :key="index" :class="{'active': selectedShapesIds.indexOf(shape.id) > -1 }">
        <span @click="selectShape(shape)" v-if="!shape.isGhost && shape.visible">{{shape.type}}</span>
    </div>
  </div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import selectShape from '../options/selectShape';
import whiteBoardService from '../whiteBoardService';
export default {
    props:{
        canvasData:{
            type:Object,
            required: true
        }
    },
computed:{
    ...mapGetters(['getDragData', 'getShapesSelected', 'getCurrentSelectedTab']),
    layers(){
        return this.getDragData;
    },
    selectedShapesIds(){
        return Object.keys(this.getShapesSelected)
    },
    tabName(){
        return this.getCurrentSelectedTab.name
    }
},
methods:{
    ...mapActions(['setShapesSelected', 'clearShapesSelected', 'setCurrentOptionSelected', 'setSelectedOptionString']),
    selectShape(shape){
        this.setSelectTool();
        this.clearShapesSelected();
        this.setShapesSelected(shape)
        selectShape.reMarkSelectedShapes.bind(this.canvasData)();
    },
    setSelectTool(){
        this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, 'selectShape')());
        this.setSelectedOptionString('selectShape');
    }
}
}
</script>

<style lang="less">
.whiteBoard_layers_container{
    position: fixed;
    left: 10px;
    bottom: 60px;
    background-color: #FFF;
    height:350px;
    width:200px;
    padding: 5px;
    overflow: auto;
    .whiteBoard_layers_layer_item{
        padding: 0 0 0 4px;
        &.active{
            background-color: #e1e4ff;
            border-radius: 4px;
        }
        span{
            cursor: pointer;
        }
    }
}
</style>