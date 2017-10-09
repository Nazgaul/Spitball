<template>
    <div>
        <div inline-flex>
            <div v-for="v in values"><input @click="radioClick($event.target.value)" :checked="value==v.id" type="radio" :id="v.id" :value="v.id" :name="model" /><label class="blue blue--text chip chip--outline" :for="v.id">{{v.name}}</label></div>
        </div>
    </div>
</template>
<script>
    export default {
        model: {
            prop: 'value',
            event:'click'
        },
        props: { value: {type:[String,Number]}, values: { type: Array}, model: { type: String }, changeCallback: { type: Function } },
        methods: {
            radioClick(value) {
                console.log(value);
                this.$emit('click', value);
                if (this.changeCallback) {
                    this.changeCallback(value);
                }
            }
        }
    }
</script>
<style scoped>
    input[type="radio"]:checked + label {
    background:black;
    }
</style>
