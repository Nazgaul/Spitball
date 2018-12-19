<template>
    <span>{{ tweeningValue }}</span>
</template>

<script>
    var TWEEN = require('./tweenJS.js');
    export default {
        name: "tweenNumber",
        props: {
            value: {
                type: Number,
                required: true
            }
        },
        data: function() {
            return {
                tweeningValue: 0
            }
        },
        watch: {
            value: function(newValue, oldValue) {
                console.log('value in tween',newValue, oldValue)
                this.tween(oldValue, newValue)
            }
        },
        mounted: function() {
            this.tween(0, this.value)
        },
        methods: {
            tween: function(startValue, endValue) {

                let self = this;
                function animate() {
                    if (TWEEN.update()) {
                        requestAnimationFrame(animate)
                    }
                }
                new TWEEN.Tween({
                    tweeningValue: startValue
                })
                    .to({
                        tweeningValue: endValue
                    }, 500)
                    .onUpdate(()=> {
                        self.tweeningValue = this.tweeningValue
                    })
                    .start();
                animate()
            }
        },
        created(){
            console.log('tween created', this.value)
        }


    }

</script>

<style scoped>

</style>