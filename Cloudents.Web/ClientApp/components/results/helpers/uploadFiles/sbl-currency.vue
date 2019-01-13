<template>
    <div>
        <label>{{label}}</label>
        <input ref="input" class="input-field" placeholder="00.00"
       :value="value"
        @input="updateValue($event.target.value)"
        @focus="selectAll"
        @blur="formatValue"
        >
    </div>
</template>

<script>
    import { currencyValidator } from "./consts";
    import { mapActions } from 'vuex';
    export default {
        props: {
            value: {
                type: Number,
                default: 0
            },
            label: {
                type: String,
                default: ''
            },
            //accepts call back function to update value in store or any other place if needed
            functionCallBacks:{
                type: Function,
                required: false
            }
        },

    mounted: function () {
        //this.formatValue()
    },
    methods: {
        updateValue: function (value) {
            var result = currencyValidator.parse(value, this.value);
            if (result.warning) {
                this.$refs.input.value = result.value;
            }
            this.$emit('input', result.value);
            //update document obj in store via passes callback function     //upload price- ...mapActions(['updateFile']),
            if(!!this.functionCallBacks){
                this.functionCallBacks({'price': result.value});
            }

        },
        formatValue: function () {
            this.$refs.input.value = currencyValidator.format(this.value);
        },
        selectAll: function (event) {
            // Workaround for Safari bug
            // http://stackoverflow.com/questions/1269722/selecting-text-on-focus-using-jquery-not-working-in-safari-and-chrome
            setTimeout(function () {
                event.target.select()
            }, 0)
        }
    }
    }
</script>

