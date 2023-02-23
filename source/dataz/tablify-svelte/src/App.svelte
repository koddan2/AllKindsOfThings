<script lang="ts">
  import Dispatch from "./lib/Dispatch.svelte";
  import * as example from "./example-data";
  let data = example.getAllData({ a: 1 });
  let maxLevel = 1;
</script>

<main>
  <div>
    <button on:click={(e) => (maxLevel -= 1)}>-</button>
    <span>{maxLevel}</span>
    <button on:click={(e) => (maxLevel += 1)}>+</button>
  </div>
  <Dispatch {data} {maxLevel} path={["$"]} />
</main>

<style>
</style>
