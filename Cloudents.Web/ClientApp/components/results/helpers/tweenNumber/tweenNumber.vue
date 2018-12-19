<template>
    <span class="tweened-number">{{ tweeningValue }}</span>
</template>

<script>
    /*
    * Receives object item as prop
    * required item.id -> integer (helps to apply specific formatter)
    * required item.data -> integer ( data to tween)
    * accept conditional prop, startValue, value to start animation from
    *
    * */

    var TWEEN = require('./tweenJS.js');
    export default {
        name: "tweenNumber",
        props: {
            startValue:{
                default: 0,
                required: false
            },
            item: {
                type: Object,
                required: true
            },
        },
        data: function() {
            return {
                tweeningValue: 0
            }
        },
        watch: {
            value: function(newValue, oldValue) {
                this.tween(oldValue, newValue)
            }
        },
        mounted: function() {
            this.tween(this.startValue, this.value)
        },
        computed: {
            value() {
                return this.item.data;
            },
            id(){
                return this.item.id
            }
        },
        methods: {
            localeNumber(val, id) { //val string
                let numericVal = parseInt(val);
                // format currency
                if (id === 2) {
                    return `$${numericVal.toLocaleString('us-EG')}`;
                    //number
                } else {
                    return numericVal.toLocaleString('us-EG');
                }
            },

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
                    }, 3000)
                    .easing(TWEEN.Easing.Linear.None) //animation frame
                    .onUpdate(function(obj) {
                        self.tweeningValue = self.localeNumber(obj.tweeningValue.toFixed(0), self.id);
                    })
                    .start();
                animate()
            }
        },



    }

</script>

<style scoped>

</style>