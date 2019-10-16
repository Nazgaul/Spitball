<template>
    <v-flex xs12 class="question_area" :class="{'has-error': error && error.errorClass}">
        <div class="textarea">
            <div class="text-block">
                <span class="error-message" v-if="error.errorClass">{{error.errorText}}</span>
                <textarea 
                    rows="9" 
                    required
                    @input="updateValue($event.target.value)"
                    :value="value" autofocus="isFocused"
                    :placeholder="'extendedTextArea_type_your_answer'" 
                    v-language:placeholder>
                </textarea>
            </div>
        </div>
    </v-flex>
</template>
<script>
export default {
    props: {
        value: {type: String},
        error: {},
        actionType: {type: String, default: 'answer'},
        isFocused: false,
        uploadUrl: {type: String},
        isAttachVisible: {type: Boolean, default: true, required: false}
    },
    data() {
        return {
            //
        };
    },
    computed: {
        setPlaceholder() {
            return `extendedTextArea_type_your_${this.actionType}`
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);
        },
        remove(file) {
            this.$refs.upload.remove(file);
        },
    },
}
</script>
<style lang="less">
@import "../../../../styles/mixin.less";

.question_area {
  &.has-error {
    .text-block{
      border: 2px solid @color-red;
    }
  }
  .textarea {
    background-color: #c7c7ee;
    padding: 8px;
    height: 233px;
    @media (max-width: @screen-xs) {
      height: 249px;
    }
    display: flex;
    position: relative;
    .text-block {
      background-color: @color-white;
      width: 100%;
      height: 100%;
      border-radius: 4px;
      position: relative;
      box-shadow: none;
      .error-message {
        color: #8b0000;
        position: absolute;
        bottom: 13.5em;
        right: .5em;
        background-color: #ffd9d9;
        padding: 0 20px;
        font-weight: 600;
        border-radius: 4px;
      }
      .answer{
        bottom: 0em;
      }
      textarea {
        resize: none;
        padding: 11px 9px 4px;
        letter-spacing: -0.3px;
        height: 177px;
        overflow-y: auto;
        outline: none;
        width: 100%;
        &.active{
          .placeholder-color(@color-white, 0.8);
        }
        @media (max-width: @screen-xs) {
          height: 192px;
        }
      }
    }
    .preview-list, .text-block textarea {
      &::-webkit-scrollbar-track {
        background-color: @color-white;
      }
      &::-webkit-scrollbar {
        height: 6px;
        width: 4px;
        background-color: @color-white;
      }
      &::-webkit-scrollbar-thumb {
        background-color: #979797;
        border: none;
      }
    }
  }
}

.question_area.small {
  @media (max-width: @screen-xs) {
    height: 220px;
  }
  textarea {
    @media (max-width: @screen-xs) {
      height: 123px;
    }
  }
}
</style>
