<template>
    <div  :class="['input-wrapper', prependInnerIcon ? 'input-wrapper-bordered': '']">
        <span v-if="!bottomError" class="error-message">{{errorTextStr}}</span>
        <v-icon v-if="prependInnerIcon" class="prepend-icon">{{prependInnerIcon}}</v-icon>
        <input :required="required" :class="['input-field', prependInnerIcon ? 'inputPrepIcon': '', {errorTextStr :'invalid'}]"
               :name="name" :type="type" :bottomError="bottomError" :disabled="disabled" :readonly="isReadOnly"
               :placeholder="placeholder"  :value="value" :hint="hint"
               @change="hideError()"
               @input="updateValue($event.target.value)" :autofocus="autofocus">
        <v-icon v-if="icon">{{icon}}</v-icon>
        <span v-if="hint"  class="input-hint">{{hint}}</span>
        <span v-if="bottomError" class="error-message relative">{{errorTextStr}}</span>
    </div>
</template>
<script>
    export default {
        props: {
            value: {type: String},
            // rules:{},
            icon: {
                type: String,
                default: ''
            },
            prependInnerIcon:{
                type: String,
                default: ''
            },
            errorMessage: {
                type: String
            },
            name: {
                type: String
            },
            type: {
                type: String
            },
            editable: {
                type: Boolean,
                default: true
            },
            placeholder: {
                type: String
            },
            autofocus: {
                type: Boolean,
            },
            focus: {
                type: Boolean,
                default: false
            },
            required:{
                type: Boolean,
                default: false
            },
            disabled:{
                type:Boolean,
                default:false
            },
            bottomError:{
                type: Boolean,
                default: false
            },
            hint:{
                default: '',
                required: false,
                type: String
            }
        },
        data: function () {
            return {
                isReadOnly: !this.editable
            };
        },
        watch: {
            focus(newVal) {
                if (newVal) {
                    this.isReadOnly = false;
                    this.$el.querySelector('.input-field').focus();
                }
            },
        },
        computed:{
            errorTextStr:{
                get(){
                    return this.errorMessage;
                },
                set(){
                }
            }
        },
        methods: {
            updateValue: function (value) {
                this.$emit('input', value);
            },
            hideError(){
                return this.errorTextStr = '';
            },
        },
        mounted() {
            if(this.autofocus){
                this.$el.querySelectorAll('.input-field')[0].focus();
            }
        },

    }
</script>

<style  lang="less">
    @import "../../../../styles/mixin.less";

    .input-wrapper {

        //with prepend icon, we need focus for wraper and not input
        &.input-wrapper-bordered{
            border-radius: 4px;
            //border: solid 1px #979797;
            .glowingBorder();
            &:focus-within{
                //border: solid 1px @colorInputFocus;
                .glowingBorderFocused();

            }
            .prepend-icon{
                padding-left: 12px;
                font-size: 18px;
                color: @colorTypeDocDefault;
            }
        }


        &.bad {
            .input-hint {
                color: @Pass-Weak-Color;
            }
        }
        &.good {
            .input-hint {
                color: @Pass-Strong-Color;
            }
        }
        &.best {
            .input-hint {
                color: @Pass-Strongest-Color;
            }
        }
        position: relative;

        .error-message {
            position: absolute;
            bottom: -22px;
            font-size: 14px;
            letter-spacing: -0.3px;
            color: #d0021b;
            &.relative {
                position: relative;
                bottom: 0;
            }
        }
        .input-hint {
            position: absolute;
            top: 16px;
            right: 18px;
            font-size: 14px;
        }
        .input-field {
            height: @registrationFieldHeight;
            width: 100%;
            border-radius: 4px;
            .glowingBorder();
            font-size: 18px;
            letter-spacing: -0.5px;
            padding: 10px;
            &.inputPrepIcon{
                border: none;
            }
            &:focus {
                outline: none;
            }
            &.invalid {
                border-color: #d0021b;
            }
        }
        .icon {
            color: @color-grey;
        }
    }
</style>