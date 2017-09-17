<template>
    <div>
        <carousel scrollPerPage="false" perPage="7">
            <slide v-for="vertical in verticals">
                <div class="round-icon">
                    <component v-bind:is="vertical.image"></component>
                </div>
                {{vertical.name}}
            </slide>
        </carousel>
    </div>
</template>


<script src="./vertical-collection.js">
</script>
<style src="./vertical-collection.less" lang="less" scoped></style>


