<template>
    <div>
        <div class="text-xs-center">
            <v-chip  color="not-selected" v-for="v in values" :key="v.id"
                    @click="radioClick(v)"
                    :selected="value==v.id">{{v.name}}  </v-chip>

        </div>
    </div>
    <!--<v-flex>
        <div inline-flex>
            <div class="text-xs-center ml-1" v-for="v in values" :key="v.id">
                <input @click="radioClick($event.target.value)" :checked="value==v.id" :ref="v.id" type="radio" :id="v.id" :value="v.id" :name="model" />
                <label class=" chip chip--outline" :for="v.id" :class="[value==v.id?'selected':'not-selected']">{{v.name}}</label>
            </div>
            <v-spacer></v-spacer>
        </div>
    </v-flex>-->
</template>
<script>
    export default {
        model: {
            prop: 'value',
            event: 'click'
        },
        data: function () {
            return { checkVal: this.value }
        },
        computed: {
            currentVal: function () { return this.checkVal || this.value }
        },
        props: {
            value: { type: [String, Number] },
            values: { type: [String, Array] },
            model: { type: String },
            changeCallback: { type: Function }
        },
        methods: {
            radioClick(value) {
                this.value = value.id;
                this.$emit('click', value);
                //if (this.changeCallback) {
                    //this.changeCallback(value);
                //}
            }
        }
    }
</script>
<style src="./radioList.less" scoped lang="less"></style>
<!--<style scoped lang="less">
   
    .not-selected--text {
        color: blueviolet;
    }
    input[type="radio"]:checked + label {
    background:black!important;
    }*/
    /*input[type="radio"] {
        display: none;
    }

    .header input[type="radio"]:checked + label {
        background: #0455a8 !important;
        color: white
    }

    .header label {
        background: #bbbbbb !important;
        color: white
    }

    .search input[type="radio"]:checked + label {
        background: #ffffff !important;
        color: #3293f6;
    }

    .search label {
        background: #eeeeee !important;
        color: #3293f6;
        border: solid 1px #3293f6;
    }

    .sub-search input[type="radio"]:checked + label {
        background: #1e59a8 !important;
        color: #ffffff;
    }

    .sub-search label {
        background: #eeeeee !important;
        color: #1e59a8;
        border: solid 1px #1e59a8;
    }*/
</style>-->
