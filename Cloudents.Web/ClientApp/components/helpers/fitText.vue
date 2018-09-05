<template>
    <div id="previewContainer" :style="container" class="text-container">
        <p id="previewText" ref="previewText" :style="text" v-html="previewText" class="scrollbar" dir="auto"></p>
    </div>
</template>

<script>
    export default {
        name: "fit-text",
        props: ['inputText', 'previewOptions'],
        watch:{
            "previewOptions.safeWidth":function(val){
                this.safeWidth=val},
            "previewOptions.previewHeight":function(val){
                this.previewHeight=val}
               },
        data: function() {
            return {
                originalFontSize: this.previewOptions.originalFontSize,
                previewHeight: this.previewOptions.previewHeight,
                previewVertOffset: this.previewOptions.previewVertOffset,
                safeWidth: this.previewOptions.safeWidth,

                container: {
                    height: '0'
                },
                text: {
                    fontSize: '18px',
                    overflow: 'hidden',
                    top: 0,
                    left: 0
                }
            }
        },

        computed: {
            previewText: function() {
                return this.nl2br(this.htmlEntities(this.inputText))
            }
        },

        mounted: function() {
            requestAnimationFrame(this.render)
        },

        methods: {
            render: function() {
                this.keepRatio();
                this.scaleText();
                requestAnimationFrame(this.render)
            },

            /**
             * Maintain the aspect ratio of the container
             */
            keepRatio: function() {
                let newHeight = Math.round(this.$el.clientWidth * (9 / 16));
                this.previewHeight = newHeight;
                this.container.height = newHeight + 'px';
            },

            /**
             * Scale the previewText element to fit the previewContainer
             */
            scaleText: function() {
                let newTextScale = 1
                let previewScale = this.previewHeight / 360

                // If the current text does not fit inside the "safeWidth" bounds of the default container, scale it
                let currentTextWidth = this.textWidth()

                if (currentTextWidth > this.safeWidth) {
                    newTextScale = this.safeWidth / currentTextWidth
                }

                // Scale text to match the actual container size
                newTextScale *= previewScale

                let newFontSize = parseInt(this.text.fontSize, 10) * newTextScale

                this.updateTextSize(newFontSize, previewScale)
            },

            /**
             * Update the font size and position of the text element in the DOM
             */
            updateTextSize: function(newFontSize, previewScale) {
                this.$refs.previewText.style['font-size'] = newFontSize + 'px'
                this.text.fontSize = newFontSize + 'px'

                let size = this.getElementSizes();
                this.text.left = (size.container.width - size.text.width) / 2 + 'px'
                this.text.top = (size.container.height - size.text.height) / 2 + (this.previewVertOffset * previewScale) + 'px'
            },

            /**
             * Get the full width of the previewText element
             */
            textWidth: function() {
                this.text.fontSize = this.originalFontSize + 'px';
                this.$refs.previewText.style['font-size'] = this.originalFontSize + 'px';
                this.$refs.previewText.style['overflow'] = 'auto';

                let textWidth = this.$refs.previewText.scrollWidth;

                this.$refs.previewText.style['overflow'] = 'hidden';
                return textWidth
            },

            /**
             * Get the total height and width for the container and text elements
             */
            getElementSizes: function() {
                return {
                    container: {
                        width: this.$el.getBoundingClientRect().width,
                        height: this.$el.getBoundingClientRect().height
                    },
                    text: {
                        width: this.$refs.previewText.getBoundingClientRect().width,
                        height: this.$refs.previewText.getBoundingClientRect().height
                    }
                }
            },

            htmlEntities: function(str) {
                return String(str).replace(/&/g, '&').replace(/</g, '<').replace(/>/g, '>').replace(/"/g, '"')
            },

            nl2br: function(string) {
                return string.replace(/(\r\n|\n\r|\r|\n)/g, "<br>")
            }
        }
    }
</script>

<style scoped>

</style>