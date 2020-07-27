<template>
    <div>
        <button id="buttonB" @click="boldText">B</button>
        <button id="buttonI" @click="italicText">I</button>
        <button id="buttonU" @click="underLine">U</button>
        <button @click="insertImage">Image</button>
        <div id="rooster" ref="editor" style="width: 500px; height: 300px; overflow: auto; border: solid 1px black"></div>

        <v-btn @click="testing">Click</v-btn>
        <div v-html="test" style="width: 500px; height: 300px; overflow: auto; border: solid 1px black"></div>
    </div>
</template>

<script>
import * as rooster from 'roosterjs';
export default {
    data() {
        return {
            contentDiv: null,
            editor: null,
            test: null,
        }
    },
    methods: {
        testing() {
            this.test = this.editor.getContent()
        },
        initEditor() {
            this.editor = rooster.createEditor(this.contentDiv)
            this.editor.setContent('<div>Welcome to <b>RoosterJs</b>!</div>');
        },
        boldText() {
            rooster.toggleBold(this.editor);
        },
        italicText() {
            rooster.toggleItalic(this.editor);
        },
        underLine() {
            rooster.toggleUnderline(this.editor)
        },
        insertImage() {
            rooster.insertImage(this.editor, 'https://static.wixstatic.com/media/a27d24_c68f7180e78246adad58d93084c9c90e~mv2.jpg/v1/fit/w_1000,h_1000,al_c,q_80/file.png')
        }
    },
    beforeDestroy() {
        this.editor.dispose();
        this.editor = null;
    },
    mounted() {
        this.$nextTick(() => {
            this.contentDiv = this.$refs.editor
            this.initEditor()
        })
    }
}
</script>

<style>

</style>