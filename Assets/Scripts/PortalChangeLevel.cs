using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalChangeLevel : MonoBehaviour
{

    [SerializeField] private float portalFindRadius;
    public LayerMask portalLayer;

    public void ChangeLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private bool CheckPortal()
    {
        Collider2D[] portal = Physics2D.OverlapCircleAll(transform.position, portalFindRadius, portalLayer);
        if(portal.Length > 0)
        {
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && CheckPortal())
        {
            Debug.Log("Checked");
            ChangeLevel();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, portalFindRadius);
    }
}
