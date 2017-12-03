<template functional>
    <!--<span class="text-xs-center">
        <v-chip color="not-selected" v-for="v in values" :key="v.id"
                @click="(!$_isDisabledButton(v.id)?radioClick(v.id):'')" :disabled="$_isDisabledButton(v.id)"
                :selected="checkVal==v.id">{{v.name}}
        <sort-arrow class="ml-2" v-if="checkVal==v.id&&model=='sort'">
                    </sort-arrow></v-chip>

    </span>-->


    <v-expansion-panel expand>
        <v-expansion-panel-content>
            <div slot="header">{{props.title}}</div>
            <div class="sort-filter">
                <v-checkbox v-if="props.values" v-for="s in props.values" @click="props.callback(s)" :label="s" :key="s" hide-details></v-checkbox>
            </div>
        </v-expansion-panel-content>
    </v-expansion-panel>




</template>
<script>
    const sortArrow = () =>import("./svg/sort-arrow.svg")
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
        components: { sortArrow },
        props: {
            value: { type: [String, Number] },
            values: { type: [String, Array] },
            model: { type: String },
            changeCallback: { type: Function },
            title:{}
        },
        methods: {
            radioClick(value) {
                this.checkVal = value;
                this.$emit('click', value);
            },

            $_isDisabledButton(id) {
                return this.$route.query.filter === 'course' && id === 'date'
            }
        }
    }
</script>
<style src="./radioList.less" lang="less"></style>
