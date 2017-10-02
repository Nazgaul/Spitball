<template>
    <div>
        <div inline-flex>
            <div v-for="v in values"><input @click="radioClick($event.target.value)" :checked="value==v.id" type="radio" :id="v.id" :value="v.id" :name="model" /><label :for="v.id">{{v.name}}</label></div>
        </div>
    </div>
</template>
<script>
    export default {
        model: {
            prop: 'value',
            event:'click'
        },
        props: { value: null, values: {}, model: { type: String }, changeCallback: { type: Function } },
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
