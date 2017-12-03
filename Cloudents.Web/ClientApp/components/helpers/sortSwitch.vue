<template functional>

    <div class="sort-switch">
        <template v-for="(o,index) in props.options">
            <input type="radio" :id="`option${index}`" @click="props.callback(o.id)"
                   name="switch" :value="o.id" :checked="props.val?props.val===o.id:index===0">
            <label :for="`option${index}`">{{o.name}}</label>
        </template>
    </div>

</template>
<style src="./sortSwitch.less" lang="less"></style>
