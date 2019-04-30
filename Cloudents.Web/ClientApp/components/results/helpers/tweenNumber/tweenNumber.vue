<template>
    <span class="tweened-number">{{ formattedValue }}</span>
</template>

<script>
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
                numObj: this.item
            }
        },
        computed: {
            formattedValue() {
                return this.localeNumber(this.numObj);
            }
        },
        methods: {
            localeNumber({data, id}) { //val string
                let numericVal = parseInt(data);
                // format currency
                if (id === 2) {
                    let dollarval = this.$options.filters.dollarVal(numericVal);
                    return `$${parseFloat(dollarval).toLocaleString('us-EG')}`;
                    //number
                } else {
                    return numericVal.toLocaleString('us-EG');
                }
            }
        }
    }

</script>

<style scoped>

</style>