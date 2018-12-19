<template>
    <span class="tweened-number">{{ tweeningValue }}</span>
</template>

<script>
    var TWEEN = require('./tweenJS.js');
    export default {
        name: "tweenNumber",
        props: {
            item: {
                type: Object,
                required: true
            },
            toLocale:{
                types: Function,
                required: false
            }
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
            this.tween(0, this.value)
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