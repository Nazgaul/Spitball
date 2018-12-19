<template>
    <span>{{ tweeningValue }}</span>
</template>

<script>
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
                this.tween(oldValue, newValue)
            }
        },
        mounted: function() {
            this.tween(0, this.value)
        },
        methods: {
            tween: function(startValue, endValue) {
                var vm = this

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
                    .onUpdate(function() {
                        vm.tweeningValue = this.tweeningValue.toFixed(0)
                    })
                    .start()
                animate()
            }
        }


    }

</script>

<style scoped>

</style>