using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetection : MonoBehaviour
{
    private List<GameObject> currentObjects = new List<GameObject>(); // Alandaki mevcut nesneleri takip eder
    public float ejectForce = 10f; // Fýrlatma kuvveti
    public float shieldEjectForce = 15f; // Kalkanýn fýrlatma kuvveti
    private bool isTriggerActive = true; // Trigger durumunu takip eder

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggerActive)
        {
            // Eðer trigger kapalýysa nesneyi kabul etme
            return;
        }

        if (currentObjects.Count > 0)
        {
            // Eðer alanda zaten bir nesne varsa yeni gelen nesneyi kalkanla fýrlat
            ActivateShield(other);
            return;
        }

        if (other.attachedRigidbody != null)
        {
            if (currentObjects.Contains(other.gameObject))
            {
                // Eðer nesne zaten alandaysa iþlem yapma
                return;
            }

            // Gelen objeyi alanýn ortasýna yerleþtir ve listeye ekle
            PlaceObjectInCenter(other);
        }
    }

    private void PlaceObjectInCenter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        rb.isKinematic = true; // Fizik etkilerini devre dýþý býrak
        Vector3 centerPosition = transform.position;
        rb.transform.position = centerPosition;
        rb.transform.rotation = Quaternion.identity; // Dik yerleþtir

        // Obje baþarýyla yerleþtirildi, listeye ekle
        currentObjects.Add(other.gameObject);
    }

    private void EjectObject(GameObject obj, float force)
    {
        // Dýþarý atmak için fizik kuvveti uygular.
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Fizik etkileþimini aç

            // Rasgele bir yön oluþtur
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f), // X ekseni
                Random.Range(0.5f, 1f), // Y ekseni
                Random.Range(-1f, 1f)  // Z ekseni
            ).normalized; // Yönü normalize ederek birim vektör yap

            rb.AddForce(randomDirection * force, ForceMode.Impulse); // Kuvvet uygula
        }
        else
        {
            // Eðer Rigidbody yoksa pozisyonu doðrudan güncelle.
            obj.transform.position += new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized * force;
        }
    }

    private void ActivateShield(Collider incomingObject)
    {
        // Kalkan gelen nesneyi farklý bir yöne fýrlatýr
        Debug.Log("Kalkan aktif: Yeni nesne fýrlatýlýyor!");
        EjectObject(incomingObject.gameObject, shieldEjectForce);
    }

    private void OnMouseDown()
    {
        if (currentObjects.Count > 0)
        {
            // Trigger'ý kapat ve içindeki nesneyi dýþarý at
            isTriggerActive = false;
            GetComponent<Collider>().isTrigger = false;

            GameObject objectToEject = currentObjects[0];
            currentObjects.Remove(objectToEject);
            EjectObject(objectToEject, ejectForce);

            // Tekrar aktif duruma getirmek için zamaný geciktirebilirsiniz
            StartCoroutine(ReactivateTrigger());
        }
    }

    private IEnumerator ReactivateTrigger()
    {
        yield return new WaitForSeconds(1f); // 1 saniye bekle
        isTriggerActive = true;
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentObjects.Contains(other.gameObject))
        {
            // Eðer nesne alaný terk ettiyse listeden çýkar
            currentObjects.Remove(other.gameObject);
        }
    }
}