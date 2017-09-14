<template>
    <div>
        asdasdasd
        {{verticals.length}}
        <ul>
            <li v-for="vertical in verticals" style="background:#000000">
                <component v-bind:is="vertical.image"></component>
                {{vertical.name}}
            </li>
        </ul>
    </div>
</template>


<script src="./vertical-collection.js">
</script>
<style src="./vertical-collection.less" lang="less" scoped></style>


