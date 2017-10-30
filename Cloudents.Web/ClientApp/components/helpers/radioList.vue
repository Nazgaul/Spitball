<template>
    <div>
        <div class="text-xs-center">
            <v-chip  color="not-selected" v-for="v in values" :key="v.id"
                    @click="radioClick(v.id)"
                    :selected="checkVal==v.id">{{v.name}} <sort-arrow class="ml-2" v-if="checkVal==v.id&&model=='sort'"></sort-arrow> </v-chip>

        </div>
    </div>
</template>
<script>
    const sortArrow=()=>import("./svg/sort-arrow.svg")
    export default {
        model: {
            prop: 'value',
            event: 'click'
        },
        data: function () {
            return {
                checkVal: this.value
            }
        },
        watch: {
            value: function (val) {
                this.checkVal = val
            }
        },
        components: { sortArrow},
        props: {
            value: { type: [String, Number] },
            values: { type: [String, Array] },
            model: { type: String },
            changeCallback: { type: Function }
        },
        methods: {
            radioClick(value) {
                this.checkVal = value;
                this.$emit('click', value);
            }
        }
    }
</script>
