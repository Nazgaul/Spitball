<template>
  <div class="writing-container ms-editor">
    <v-icon class="clearBtn" @click="clear">sbf-clearAll-icon</v-icon>
    <v-icon class="clearBtn" @click="undo">sbf-undo</v-icon>
    <div class="writing-container editor-context" touch-action="none" ref="editor"></div>
    <input style="visibility:hidden;" :value="latexSyntax" :class="[helperClass, helperObj.id]"/>
    <vue-mathjax class="writing-container-result" :formula="`$$${latexSyntax}$$`"></vue-mathjax>
  </div>
</template>

<script>
import * as MyScriptJS from 'myscript';
export default {
  data(){
    return{
      latexSyntax: '',
    }
  },
  props:{
    helperObj:{
      type: Object,
      required: true
    },
    helperClass:{

    }
  },
methods:{
    convert(){
      let editorElm = this.$refs['editor'];
      editorElm.editor.convert();
    },
    clear(){
      let editorElm = this.$refs['editor'];
      editorElm.editor.clear();
      this.latexSyntax = '';
    },
    undo(){
      let editorElm = this.$refs['editor'];
      editorElm.editor.undo();
    }
  },
  mounted() {
    let self = this;
    // Fired every second, should always be true
    // eslint-disable-next-line
    console.log("Mounted " + this.$refs['editor']);
    MyScriptJS.register(this.$refs.editor, {
      recognitionParams: {
        type: 'MATH',
        protocol: 'WEBSOCKET',
        apiVersion: 'V4',
        server: {
          scheme: 'https',
          host: 'webdemoapi.myscript.com',
          applicationKey: 'f18b4c28-ce4d-4379-8a30-f68842e8fe23',
          hmacKey: '7a6a78a6-446a-4a9a-a79d-16d50dc86996',
        },
      },
      v4: {
        math: {
          solver: {
            enable: false
          }
        }
      },
    });


  this.$refs.editor.addEventListener('exported', (event)=> {
        let latexResult = event.detail.exports['application/x-latex'];
        self.latexSyntax = latexResult;
        self.helperObj.text =  latexResult;
    });

  },
}
</script>

<style lang="less">
@width: 600px;
@height: 200px;

@keyframes spinner {
  to {transform: rotate(360deg);}
}

.ms-editor{
    position: relative;
    z-index: 20;
    color: #1a9fff;
    
    .editor-context canvas{
        z-index: 15;
        position: absolute;
        left: 0;
        top: 0;
        height: 100%;
        width: 100%;
    }
    .editor-context svg{
        z-index: 10;
        position: absolute;
        left: 0;
        top: 0;
        height: 100%;
        width: 100%;
        pointer-events: none;
        &[data-layer=BACKGROUND]{
            z-index: 9;
        }
    }
    .writing-container-result{
      position: relative;
      min-height: 80px;
      display: block;
        .MathJax_SVG{
          font-size: 170% !important;
        }
      }
}
.writing-container {
  min-height: @height;
  min-width: @width;
  &.editor-context {
    &.ms-editor{
      background:#eeeeee;
      .loader {
        max-height: @height;
        max-width: @width;
        background: #f8f8f8;
        pointer-events: none;
        &:before {
          content: '';
          box-sizing: border-box;
          position: absolute;
          top: 50%;
          left: 50%;
          width: 30px;
          height: 30px;
          margin-top: -15px;
          margin-left: -15px;
          border-radius: 50%;
          border: 1px solid #ccc;
          border-top-color: #07d;
          animation: spinner .6s linear infinite;
        }
      }
      .error-msg{
        max-height: @height;
        max-width: @width;
      }
      
    }
  }
  
  .clearBtn{
    color:#000;
    &:hover{
      color:#2a79ff;
    }
  }
}
</style>