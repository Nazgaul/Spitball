<template>
    <div>
        <div class="text-xs-center">
            <v-chip  color="not-selected" v-for="v in values" :key="v.id"
                    @click="radioClick(v.id)"
                    :selected="checkVal==v.id">{{v.name}}  </v-chip>

        </div>
    </div>
</template>
<script>
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
