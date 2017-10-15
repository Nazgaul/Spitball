<template>
    <v-flex>
        <div inline-flex>
            <div class="text-xs-center ml-1" v-for="v in values"><input @click="radioClick($event.target.value)" :checked="checkVal==v.id" :ref="v.id" type="radio" :id="v.id" :value="v.id" :name="model" /><label class=" chip chip--outline" :for="v.id" :class="[checkVal==v.id?'selected':'not-selected']">{{v.name}}</label></div>
            <v-spacer></v-spacer>
        </div>
    </v-flex>
</template>
<script>
    export default {
        model: {
            prop: 'value',
            event:'click'
        },
        data: function () {
            return {checkVal:this.value}
        },
        props: { value: { type: [String, Number] }, values: { type: [String, Array] }, model: { type: String }, changeCallback: { type: Function }},
        methods: {
            radioClick(value) {
                this.checkVal = value;
                this.$emit('click', value);
                if (this.changeCallback) {
                    this.changeCallback(value);
                }
            }
        }
    }
</script>
<style scoped>
    /*input[type="radio"]:checked + label {
    background:black!important;
    }*/
    input[type="radio"] {
         display:none;
    }

    .header .selected {
        background: #0455a8!important;
        color:white
    }

    .header .not-selected {
        background: #bbbbbb!important;
        color:white
    }

    .search .selected {
        background: #ffffff !important;
        color: #3293f6;
    }

    .search .not-selected {
        background: #eeeeee !important;
        color: #3293f6;
        border: solid 1px #3293f6;
    }

    .sub-search .selected {
        background: #1e59a8 !important;
        color: #ffffff;
    }

    .sub-search .not-selected {
        background: #eeeeee !important;
        color: #1e59a8;
        border: solid 1px #1e59a8;
    }
</style>
