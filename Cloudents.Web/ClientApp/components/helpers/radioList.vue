<template>
    <div>
        <div inline-flex>
            <div v-for="v in values"><input @click="radioClick" type="radio" :id="v.id" :value="v.id" :name="model" /><label :for="v.id">{{v.name}}</label></div>
        </div>
    </div>
</template>
<script>
    export default {
        props: { values: {}, model: { type: String }, changeCallback: { type: Function } },
        methods: {
            radioClick(e) {
                console.log(e);
                this.$emit('update:' + this.model, e.target.value);
                if (this.changeCallback) {
                    this.changeCallback(e);
                }
            }
        }
    }
</script>
